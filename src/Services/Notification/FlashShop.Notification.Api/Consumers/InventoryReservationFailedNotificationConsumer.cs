using FlashShop.MessageContracts.Events;
using FlashShop.Notification.Api.Services;
using MassTransit;

namespace FlashShop.Notification.Api.Consumers;

public class InventoryReservationFailedNotificationConsumer : IConsumer<InventoryReservationFailedEvent>
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<InventoryReservationFailedNotificationConsumer> _logger;

    public InventoryReservationFailedNotificationConsumer(INotificationService notificationService, ILogger<InventoryReservationFailedNotificationConsumer> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<InventoryReservationFailedEvent> context)
    {
        var msg = context.Message;
        _logger.LogInformation("Sending out of stock notification for order {OrderNumber}", msg.OrderNumber);

        await _notificationService.SendNotificationAsync("OrderOutOfStock", msg.UserEmail, new Dictionary<string, string>
        {
            ["OrderNumber"] = msg.OrderNumber,
            ["Reason"] = msg.Reason
        });
    }
}
