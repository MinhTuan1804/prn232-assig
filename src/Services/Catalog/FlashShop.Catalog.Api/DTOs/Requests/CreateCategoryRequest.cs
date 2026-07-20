using System.ComponentModel.DataAnnotations;

namespace FlashShop.Catalog.Api.DTOs.Requests;

public class CreateCategoryRequest
{
    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }
}
