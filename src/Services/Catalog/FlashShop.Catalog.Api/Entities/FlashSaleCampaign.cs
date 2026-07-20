namespace FlashShop.Catalog.Api.Entities;

public class FlashSaleCampaign
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Status { get; set; } = "Scheduled";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<FlashSaleItem> Items { get; set; } = new List<FlashSaleItem>();
}
