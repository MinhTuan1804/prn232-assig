using FlashShop.MessageContracts.Events;
using FlashShop.Notification.Api.Services;
using MassTransit;

namespace FlashShop.Notification.Api.Consumers;

public class InventoryReservedNotificationConsumer : IConsumer<InventoryReservedEvent>
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<InventoryReservedNotificationConsumer> _logger;

    public InventoryReservedNotificationConsumer(INotificationService notificationService, ILogger<InventoryReservedNotificationConsumer> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<InventoryReservedEvent> context)
    {
        var msg = context.Message;
        _logger.LogInformation("Sending awaiting payment notification for order {OrderNumber}", msg.OrderNumber);

        await _notificationService.SendNotificationAsync("OrderAwaitingPayment", msg.UserEmail, new Dictionary<string, string>
        {
            ["OrderNumber"] = msg.OrderNumber,
            ["TotalAmount"] = msg.TotalAmount.ToString("N0"),
            ["PaymentDeadline"] = msg.PaymentDeadline.ToString("yyyy-MM-dd HH:mm:ss")
        });
    }
}
