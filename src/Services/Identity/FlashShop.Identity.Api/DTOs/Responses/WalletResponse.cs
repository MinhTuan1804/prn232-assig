namespace FlashShop.Identity.Api.DTOs.Responses;

public class WalletResponse
{
    public decimal Balance { get; set; }
    public string Currency { get; set; } = string.Empty;
    public DateTime UpdatedAt { get; set; }
}

public class WalletTransactionResponse
{
    public Guid Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal BalanceAfter { get; set; }
    public string? ReferenceId { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}
