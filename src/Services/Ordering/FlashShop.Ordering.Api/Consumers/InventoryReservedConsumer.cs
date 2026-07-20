using FlashShop.MessageContracts.Events;
using FlashShop.Ordering.Api.Data;
using FlashShop.Shared.Constants;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace FlashShop.Ordering.Api.Consumers;

public class InventoryReservedConsumer : IConsumer<InventoryReservedEvent>
{
    private readonly OrderingDbContext _dbContext;
    private readonly ILogger<InventoryReservedConsumer> _logger;

    public InventoryReservedConsumer(OrderingDbContext dbContext, ILogger<InventoryReservedConsumer> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<InventoryReservedEvent> context)
    {
        var order = await _dbContext.Orders
            .FirstOrDefaultAsync(o => o.Id == context.Message.OrderId);

        if (order == null)
        {
            _logger.LogWarning("Order {OrderId} not found for InventoryReservedEvent", context.Message.OrderId);
            return;
        }

        order.Status = OrderStatus.AwaitingPayment;
        order.PaymentDeadline = context.Message.PaymentDeadline;
        order.UpdatedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Order {OrderNumber} updated to AwaitingPayment", order.OrderNumber);
    }
}
