using System.ComponentModel.DataAnnotations;

namespace FlashShop.Catalog.Api.DTOs.Requests;

public class CreateFlashSaleRequest
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public DateTime StartTime { get; set; }

    [Required]
    public DateTime EndTime { get; set; }

    [Required]
    [MinLength(1, ErrorMessage = "At least one item is required.")]
    public List<FlashSaleItemRequest> Items { get; set; } = new();
}

public class FlashSaleItemRequest
{
    [Required]
    public Guid ProductId { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal FlashSalePrice { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int FlashSaleQuantity { get; set; }

    [Range(1, int.MaxValue)]
    public int MaxPerUser { get; set; } = 1;
}
