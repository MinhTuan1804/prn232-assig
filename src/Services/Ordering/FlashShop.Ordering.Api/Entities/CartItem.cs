namespace FlashShop.Ordering.Api.Entities;

public class CartItem
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; } = 1;
    public string? ImageUrl { get; set; }
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
}
