namespace FlashShop.MessageContracts.Events;

public record InventoryReservedEvent
{
    public Guid OrderId { get; init; }
    public string OrderNumber { get; init; } = string.Empty;
    public string UserEmail { get; init; } = string.Empty;
    public decimal TotalAmount { get; init; }
    public DateTime PaymentDeadline { get; init; }
}
