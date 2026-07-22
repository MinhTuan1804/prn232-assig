using FlashShop.Identity.Api.Data;
using FlashShop.Identity.Api.DTOs.Requests;
using FlashShop.Identity.Api.DTOs.Responses;
using FlashShop.Identity.Api.Entities;
using FlashShop.Shared.Common;
using FlashShop.Shared.Constants;
using FlashShop.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace FlashShop.Identity.Api.Services;

public class WalletService : IWalletService
{
    private readonly IdentityDbContext _dbContext;

    public WalletService(IdentityDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<WalletResponse> GetBalanceAsync(Guid userId)
    {
        var wallet = await GetWalletByUserIdAsync(userId);
        return MapToResponse(wallet);
    }

    public async Task<WalletResponse> TopUpAsync(Guid userId, decimal amount, bool checkLock = true)
    {
        var wallet = await GetWalletByUserIdAsync(userId, checkLock);

        wallet.Balance += amount;
        wallet.UpdatedAt = DateTime.UtcNow;

        _dbContext.WalletTransactions.Add(new WalletTransaction
        {
            Id = Guid.NewGuid(),
            WalletId = wallet.Id,
            Type = WalletTransactionType.TopUp,
            Amount = amount,
            BalanceAfter = wallet.Balance,
            Description = $"Nạp tiền {amount:N0} VND vào ví",
            CreatedAt = DateTime.UtcNow
        });

        await _dbContext.SaveChangesAsync();
        return MapToResponse(wallet);
    }

    public async Task<WalletResponse> ProcessPaymentAsync(WalletPaymentRequest request)
    {
        var wallet = await GetWalletByUserIdAsync(request.UserId);

        // Ensure sufficient balance for payment
        if (wallet.Balance < request.Amount)
        {
            throw new BadRequestException("Số dư ví FlashPay không đủ để thanh toán. Vui lòng nạp thêm tiền vào ví.");
        }

        wallet.Balance -= request.Amount;
        wallet.UpdatedAt = DateTime.UtcNow;

        _dbContext.WalletTransactions.Add(new WalletTransaction
        {
            Id = Guid.NewGuid(),
            WalletId = wallet.Id,
            Type = WalletTransactionType.Payment,
            Amount = -request.Amount,
            BalanceAfter = wallet.Balance,
            ReferenceId = request.OrderId,
            Description = request.Description ?? $"Thanh toán đơn hàng {request.OrderId}",
            CreatedAt = DateTime.UtcNow
        });

        await _dbContext.SaveChangesAsync();
        return MapToResponse(wallet);
    }

    public async Task<WalletResponse> ProcessRefundAsync(Guid userId, decimal amount, string orderId)
    {
        var wallet = await GetWalletByUserIdAsync(userId);

        wallet.Balance += amount;
        wallet.UpdatedAt = DateTime.UtcNow;

        _dbContext.WalletTransactions.Add(new WalletTransaction
        {
            Id = Guid.NewGuid(),
            WalletId = wallet.Id,
            Type = WalletTransactionType.Refund,
            Amount = amount,
            BalanceAfter = wallet.Balance,
            ReferenceId = orderId,
            Description = $"Hoàn tiền đơn hàng {orderId}",
            CreatedAt = DateTime.UtcNow
        });

        await _dbContext.SaveChangesAsync();
        return MapToResponse(wallet);
    }

    public async Task<PagedResult<WalletTransactionResponse>> GetTransactionsAsync(Guid userId, int page, int pageSize)
    {
        var wallet = await GetWalletByUserIdAsync(userId);

        var query = _dbContext.WalletTransactions
            .Where(t => t.WalletId == wallet.Id)
            .AsNoTracking();

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderByDescending(t => t.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(t => new WalletTransactionResponse
            {
                Id = t.Id,
                Type = t.Type,
                Amount = t.Amount,
                BalanceAfter = t.BalanceAfter,
                ReferenceId = t.ReferenceId,
                Description = t.Description,
                CreatedAt = t.CreatedAt
            })
            .ToListAsync();

        return new PagedResult<WalletTransactionResponse>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    private async Task<Wallet> GetWalletByUserIdAsync(Guid userId, bool checkLock = true)
    {
        if (checkLock)
        {
            var userObj = await _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId);
            if (userObj != null && !userObj.IsActive)
            {
                throw new BadRequestException("Tài khoản và Ví FlashPay của bạn hiện đang bị KHOÁ. Vui lòng liên hệ Quản trị viên để được hỗ trợ!");
            }
        }

        var wallet = await _dbContext.Wallets.FirstOrDefaultAsync(w => w.UserId == userId);
        if (wallet is null)
        {
            wallet = new Wallet
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Balance = 0m,
                Currency = "VND",
                UpdatedAt = DateTime.UtcNow
            };

            _dbContext.Wallets.Add(wallet);
            await _dbContext.SaveChangesAsync();
        }
        return wallet;
    }

    private static WalletResponse MapToResponse(Wallet wallet) => new()
    {
        Balance = wallet.Balance,
        Currency = wallet.Currency,
        UpdatedAt = wallet.UpdatedAt
    };
}
