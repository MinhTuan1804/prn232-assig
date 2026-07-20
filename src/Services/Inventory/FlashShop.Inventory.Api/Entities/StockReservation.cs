namespace FlashShop.Inventory.Api.Entities;

public class StockReservation
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public Guid OrderId { get; set; }
    public int Quantity { get; set; }
    public string Status { get; set; } = "Reserved";
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
