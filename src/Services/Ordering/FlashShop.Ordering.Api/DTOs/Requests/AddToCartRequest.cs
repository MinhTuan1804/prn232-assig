using System.ComponentModel.DataAnnotations;

namespace FlashShop.Ordering.Api.DTOs.Requests;

public class AddToCartRequest
{
    [Required]
    public string ProductId { get; set; } = string.Empty;

    [Required, MaxLength(300)]
    public string ProductName { get; set; } = string.Empty;

    [Required, Range(0.01, double.MaxValue)]
    public decimal UnitPrice { get; set; }

    [Range(1, 100)]
    public int Quantity { get; set; } = 1;

    [MaxLength(2000)]
    public string? ImageUrl { get; set; }
}
