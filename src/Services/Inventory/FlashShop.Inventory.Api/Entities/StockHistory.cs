namespace FlashShop.Inventory.Api.Entities;

public class StockHistory
{
    public long Id { get; set; }
    public Guid ProductId { get; set; }
    public string ChangeType { get; set; } = string.Empty;
    public int QuantityChange { get; set; }
    public string? ReferenceId { get; set; }
    public DateTime CreatedAt { get; set; }
}
