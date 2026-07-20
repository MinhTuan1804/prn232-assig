namespace FlashShop.Inventory.Api.Entities;

public class Stock
{
    public Guid ProductId { get; set; }
    public int AvailableQuantity { get; set; }
    public int ReservedQuantity { get; set; }
    public int MinThreshold { get; set; } = 10;
    public byte[] RowVersion { get; set; } = null!;
    public DateTime UpdatedAt { get; set; }
}
