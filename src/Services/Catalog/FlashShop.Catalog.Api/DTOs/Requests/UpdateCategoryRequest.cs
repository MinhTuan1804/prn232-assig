using System.ComponentModel.DataAnnotations;

namespace FlashShop.Catalog.Api.DTOs.Requests;

public class UpdateCategoryRequest
{
    [MaxLength(150)]
    public string? Name { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }
}
