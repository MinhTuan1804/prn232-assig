using System.ComponentModel.DataAnnotations;

namespace FlashShop.Catalog.Api.DTOs.Requests;

public class CreateProductRequest
{
    [Required]
    [MaxLength(300)]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
    public decimal Price { get; set; }

    public string? ImageUrl { get; set; }

    [Required]
    public int CategoryId { get; set; }

    public List<string> ImageUrls { get; set; } = new();
}
