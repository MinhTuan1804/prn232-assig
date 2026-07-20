namespace FlashShop.MessageContracts.Events;

public record OrderPaidEvent
{
    public Guid OrderId { get; init; }
    public string OrderNumber { get; init; } = string.Empty;
    public Guid UserId { get; init; }
    public string UserEmail { get; init; } = string.Empty;
    public decimal TotalAmount { get; init; }
}
