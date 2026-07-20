using FlashShop.MessageContracts.Events;
using FlashShop.Notification.Api.Services;
using MassTransit;

namespace FlashShop.Notification.Api.Consumers;

public class OrderPaidNotificationConsumer : IConsumer<OrderPaidEvent>
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<OrderPaidNotificationConsumer> _logger;

    public OrderPaidNotificationConsumer(INotificationService notificationService, ILogger<OrderPaidNotificationConsumer> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<OrderPaidEvent> context)
    {
        var msg = context.Message;
        _logger.LogInformation("Sending payment confirmed notification for order {OrderNumber}", msg.OrderNumber);

        await _notificationService.SendNotificationAsync("OrderPaid", msg.UserEmail, new Dictionary<string, string>
        {
            ["OrderNumber"] = msg.OrderNumber,
            ["TotalAmount"] = msg.TotalAmount.ToString("N0")
        });
    }
}
