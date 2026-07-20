namespace FlashShop.MessageContracts.Events;

public record OrderCreatedEvent
{
    public Guid OrderId { get; init; }
    public string OrderNumber { get; init; } = string.Empty;
    public Guid UserId { get; init; }
    public string UserEmail { get; init; } = string.Empty;
    public bool IsFlashSaleOrder { get; init; }
    public List<OrderItemDetail> Items { get; init; } = new();
}

public record OrderItemDetail
{
    public Guid ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
}
