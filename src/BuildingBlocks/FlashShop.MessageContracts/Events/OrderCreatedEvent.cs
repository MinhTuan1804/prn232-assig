namespace FlashShop.MessageContracts.Events;

/// <summary>
/// Integration event published by Ordering after checkout. Inventory consumes
/// it to reserve stock and Notification uses subsequent reservation events.
/// </summary>
public record OrderCreatedEvent
{
    public Guid OrderId { get; init; }
    public string OrderNumber { get; init; } = string.Empty;
    public Guid UserId { get; init; }
    public string UserEmail { get; init; } = string.Empty;
    public bool IsFlashSaleOrder { get; init; }
    public List<OrderItemDetail> Items { get; init; } = new();
}

/// <summary>Immutable product snapshot carried with an order event.</summary>
public record OrderItemDetail
{
    public Guid ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
}
