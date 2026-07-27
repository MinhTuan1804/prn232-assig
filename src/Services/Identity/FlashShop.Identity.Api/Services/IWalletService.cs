using FlashShop.Identity.Api.DTOs.Requests;
using FlashShop.Identity.Api.DTOs.Responses;
using FlashShop.Shared.Common;

namespace FlashShop.Identity.Api.Services;

/// <summary>Owns wallet balances and their auditable transaction history.</summary>
public interface IWalletService
{
    /// <summary>Gets the current balance for a user wallet.</summary>
    Task<WalletResponse> GetBalanceAsync(Guid userId);

    /// <summary>Adds funds to a wallet, optionally respecting its lock state.</summary>
    Task<WalletResponse> TopUpAsync(Guid userId, decimal amount, bool checkLock = true);

    /// <summary>Atomically deducts an order payment from the requested wallet.</summary>
    Task<WalletResponse> ProcessPaymentAsync(WalletPaymentRequest request);

    /// <summary>Returns funds for a cancelled order and records the source order.</summary>
    Task<WalletResponse> ProcessRefundAsync(Guid userId, decimal amount, string orderId);

    /// <summary>Returns the wallet ledger in pages for account history.</summary>
    Task<PagedResult<WalletTransactionResponse>> GetTransactionsAsync(Guid userId, int page, int pageSize);
}
