namespace FlashShop.Catalog.Api.DTOs.Responses;

public class FlashSaleCampaignResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Status { get; set; } = string.Empty;
    public List<FlashSaleItemResponse> Items { get; set; } = new();
}

public class FlashSaleItemResponse
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? ProductImageUrl { get; set; }
    public decimal OriginalPrice { get; set; }
    public decimal FlashSalePrice { get; set; }
    public int FlashSaleQuantity { get; set; }
    public int SoldQuantity { get; set; }
    public int StockQuantity { get; set; }
    public int MaxPerUser { get; set; }
}
