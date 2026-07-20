using FlashShop.MessageContracts.Events;
using FlashShop.Ordering.Api.Data;
using FlashShop.Shared.Constants;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace FlashShop.Ordering.Api.Consumers;

public class InventoryReservationFailedConsumer : IConsumer<InventoryReservationFailedEvent>
{
    private readonly OrderingDbContext _dbContext;
    private readonly ILogger<InventoryReservationFailedConsumer> _logger;

    public InventoryReservationFailedConsumer(OrderingDbContext dbContext, ILogger<InventoryReservationFailedConsumer> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<InventoryReservationFailedEvent> context)
    {
        var order = await _dbContext.Orders
            .FirstOrDefaultAsync(o => o.Id == context.Message.OrderId);

        if (order == null)
        {
            _logger.LogWarning("Order {OrderId} not found for InventoryReservationFailedEvent", context.Message.OrderId);
            return;
        }

        order.Status = OrderStatus.Cancelled;
        order.CancelledAt = DateTime.UtcNow;
        order.CancelReason = context.Message.Reason;
        order.UpdatedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Order {OrderNumber} cancelled due to: {Reason}", order.OrderNumber, context.Message.Reason);
    }
}
