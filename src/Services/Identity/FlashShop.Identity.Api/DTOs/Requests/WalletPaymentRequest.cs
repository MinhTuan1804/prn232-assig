using System.ComponentModel.DataAnnotations;

namespace FlashShop.Identity.Api.DTOs.Requests;

public class WalletPaymentRequest
{
    [Required]
    public Guid UserId { get; set; }

    [Required, Range(0.01, double.MaxValue)]
    public decimal Amount { get; set; }

    [Required]
    public string OrderId { get; set; } = string.Empty;

    public string? Description { get; set; }
}
