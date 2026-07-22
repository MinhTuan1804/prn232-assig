namespace FlashShop.Catalog.Api.DTOs.Responses;

public class ProductResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public List<ProductImageResponse> Images { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int StockQuantity { get; set; } = 15;
    public bool IsSoldOut => StockQuantity <= 0;
    public decimal? FlashSalePrice { get; set; }
    public DateTime? FlashSaleEndTime { get; set; }
}

public class ProductImageResponse
{
    public int Id { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public int SortOrder { get; set; }
}
