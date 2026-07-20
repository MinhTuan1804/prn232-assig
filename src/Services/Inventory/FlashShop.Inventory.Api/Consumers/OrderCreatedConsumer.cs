using FlashShop.Inventory.Api.Services;
using FlashShop.MessageContracts.Events;
using MassTransit;

namespace FlashShop.Inventory.Api.Consumers;

public class OrderCreatedConsumer : IConsumer<OrderCreatedEvent>
{
    private readonly IReservationService _reservationService;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<OrderCreatedConsumer> _logger;

    public OrderCreatedConsumer(
        IReservationService reservationService,
        IPublishEndpoint publishEndpoint,
        ILogger<OrderCreatedConsumer> logger)
    {
        _reservationService = reservationService;
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Processing inventory reservation for order {OrderId}", message.OrderId);

        var success = await _reservationService.ReserveStockAsync(message.OrderId, message.Items);

        if (success)
        {
            var totalAmount = message.Items.Sum(i => i.Quantity * i.UnitPrice);

            await _publishEndpoint.Publish(new InventoryReservedEvent
            {
                OrderId = message.OrderId,
                OrderNumber = message.OrderNumber,
                UserEmail = message.UserEmail,
                TotalAmount = totalAmount,
                PaymentDeadline = DateTime.UtcNow.AddMinutes(15)
            });

            _logger.LogInformation("Inventory reserved for order {OrderId}", message.OrderId);
        }
        else
        {
            await _publishEndpoint.Publish(new InventoryReservationFailedEvent
            {
                OrderId = message.OrderId,
                OrderNumber = message.OrderNumber,
                UserEmail = message.UserEmail,
                Reason = "Insufficient stock"
            });

            _logger.LogWarning("Inventory reservation failed for order {OrderId}", message.OrderId);
        }
    }
}
