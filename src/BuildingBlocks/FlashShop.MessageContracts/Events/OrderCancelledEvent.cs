namespace FlashShop.MessageContracts.Events;

public record OrderCancelledEvent
{
    public Guid OrderId { get; init; }
    public string OrderNumber { get; init; } = string.Empty;
    public string UserEmail { get; init; } = string.Empty;
    public string Reason { get; init; } = string.Empty;
}
