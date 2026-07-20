using FlashShop.Inventory.Api.Services;
using FlashShop.MessageContracts.Events;
using MassTransit;

namespace FlashShop.Inventory.Api.Consumers;

public class OrderCancelledConsumer : IConsumer<OrderCancelledEvent>
{
    private readonly IReservationService _reservationService;
    private readonly ILogger<OrderCancelledConsumer> _logger;

    public OrderCancelledConsumer(IReservationService reservationService, ILogger<OrderCancelledConsumer> logger)
    {
        _reservationService = reservationService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<OrderCancelledEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Releasing reservation for order {OrderId}", message.OrderId);

        await _reservationService.ReleaseReservationAsync(message.OrderId);

        _logger.LogInformation("Reservation released for order {OrderId}", message.OrderId);
    }
}
