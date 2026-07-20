using System.ComponentModel.DataAnnotations;

namespace FlashShop.Catalog.Api.DTOs.Requests;

public class UpdateProductRequest
{
    [MaxLength(300)]
    public string? Name { get; set; }

    public string? Description { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
    public decimal? Price { get; set; }

    public string? ImageUrl { get; set; }

    public int? CategoryId { get; set; }

    public List<string>? ImageUrls { get; set; }
}
