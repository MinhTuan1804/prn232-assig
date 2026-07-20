using System.ComponentModel.DataAnnotations;

namespace FlashShop.Ordering.Api.DTOs.Requests;

public class CheckoutRequest
{
    [Required, MaxLength(500)]
    public string ShippingAddress { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string RecipientName { get; set; } = string.Empty;

    [Required, MaxLength(20)]
    public string RecipientPhone { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Notes { get; set; }

    public bool IsFlashSaleOrder { get; set; }
}
