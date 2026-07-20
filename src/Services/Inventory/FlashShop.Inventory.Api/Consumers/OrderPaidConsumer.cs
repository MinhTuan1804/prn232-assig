using FlashShop.Inventory.Api.Services;
using FlashShop.MessageContracts.Events;
using MassTransit;

namespace FlashShop.Inventory.Api.Consumers;

public class OrderPaidConsumer : IConsumer<OrderPaidEvent>
{
    private readonly IReservationService _reservationService;
    private readonly ILogger<OrderPaidConsumer> _logger;

    public OrderPaidConsumer(IReservationService reservationService, ILogger<OrderPaidConsumer> logger)
    {
        _reservationService = reservationService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<OrderPaidEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Confirming reservation for order {OrderId}", message.OrderId);

        await _reservationService.ConfirmReservationAsync(message.OrderId);

        _logger.LogInformation("Reservation confirmed for order {OrderId}", message.OrderId);
    }
}
