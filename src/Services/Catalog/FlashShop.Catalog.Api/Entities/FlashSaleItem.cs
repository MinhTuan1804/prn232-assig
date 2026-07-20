namespace FlashShop.Catalog.Api.Entities;

public class FlashSaleItem
{
    public int Id { get; set; }
    public Guid CampaignId { get; set; }
    public Guid ProductId { get; set; }
    public decimal FlashSalePrice { get; set; }
    public int FlashSaleQuantity { get; set; }
    public int SoldQuantity { get; set; } = 0;
    public int MaxPerUser { get; set; } = 1;

    public FlashSaleCampaign Campaign { get; set; } = null!;
    public Product Product { get; set; } = null!;
}
