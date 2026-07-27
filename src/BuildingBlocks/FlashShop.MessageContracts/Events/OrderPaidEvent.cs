namespace FlashShop.MessageContracts.Events;

/// <summary>
/// Integration event published after a successful wallet payment. Consumers
/// use it to confirm inventory reservations and send payment notifications.
/// </summary>
public record OrderPaidEvent
{
    public Guid OrderId { get; init; }
    public string OrderNumber { get; init; } = string.Empty;
    public Guid UserId { get; init; }
    public string UserEmail { get; init; } = string.Empty;
    public decimal TotalAmount { get; init; }
}
