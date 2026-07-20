using FlashShop.MessageContracts.Events;
using FlashShop.Notification.Api.Services;
using MassTransit;

namespace FlashShop.Notification.Api.Consumers;

public class OrderCancelledNotificationConsumer : IConsumer<OrderCancelledEvent>
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<OrderCancelledNotificationConsumer> _logger;

    public OrderCancelledNotificationConsumer(INotificationService notificationService, ILogger<OrderCancelledNotificationConsumer> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<OrderCancelledEvent> context)
    {
        var msg = context.Message;
        _logger.LogInformation("Sending cancellation notification for order {OrderNumber}", msg.OrderNumber);

        await _notificationService.SendNotificationAsync("OrderCancelled", msg.UserEmail, new Dictionary<string, string>
        {
            ["OrderNumber"] = msg.OrderNumber,
            ["Reason"] = msg.Reason
        });
    }
}
