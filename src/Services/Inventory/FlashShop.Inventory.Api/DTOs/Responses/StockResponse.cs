namespace FlashShop.Inventory.Api.DTOs.Responses;

public record StockResponse
{
    public Guid ProductId { get; init; }
    public int AvailableQuantity { get; init; }
    public int ReservedQuantity { get; init; }
    public int TotalQuantity => AvailableQuantity + ReservedQuantity;
    public int MinThreshold { get; init; }
    public bool IsLowStock => AvailableQuantity <= MinThreshold;
    public DateTime UpdatedAt { get; init; }
}
