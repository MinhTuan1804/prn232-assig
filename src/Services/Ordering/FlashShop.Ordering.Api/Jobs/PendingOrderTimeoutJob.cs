using FlashShop.Ordering.Api.Services;

namespace FlashShop.Ordering.Api.Jobs;

public class PendingOrderTimeoutJob
{
    private readonly IServiceScopeFactory _scopeFactory;

    public PendingOrderTimeoutJob(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task Execute()
    {
        using var scope = _scopeFactory.CreateScope();
        var orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();
        await orderService.CancelExpiredOrdersAsync();
    }
}
