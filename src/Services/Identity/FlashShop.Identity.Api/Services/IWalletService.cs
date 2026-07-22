using FlashShop.Identity.Api.DTOs.Requests;
using FlashShop.Identity.Api.DTOs.Responses;
using FlashShop.Shared.Common;

namespace FlashShop.Identity.Api.Services;

public interface IWalletService
{
    Task<WalletResponse> GetBalanceAsync(Guid userId);
    Task<WalletResponse> TopUpAsync(Guid userId, decimal amount, bool checkLock = true);
    Task<WalletResponse> ProcessPaymentAsync(WalletPaymentRequest request);
    Task<WalletResponse> ProcessRefundAsync(Guid userId, decimal amount, string orderId);
    Task<PagedResult<WalletTransactionResponse>> GetTransactionsAsync(Guid userId, int page, int pageSize);
}
