using FlashShop.Notification.Api.Services;

namespace FlashShop.Notification.Api.Jobs;

public class DailySalesReportJob
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConfiguration _configuration;

    public DailySalesReportJob(IServiceScopeFactory scopeFactory, IConfiguration configuration)
    {
        _scopeFactory = scopeFactory;
        _configuration = configuration;
    }

    public async Task Execute()
    {
        using var scope = _scopeFactory.CreateScope();
        var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
        var httpClientFactory = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<DailySalesReportJob>>();

        var today = DateTime.UtcNow.ToString("yyyy-MM-dd");
        var adminEmail = _configuration["AdminEmail"] ?? "admin@flashshop.com";

        try
        {
            var orderingClient = httpClientFactory.CreateClient("OrderingService");
            var response = await orderingClient.GetAsync($"/api/orders?page=1&pageSize=1");

            var placeholders = new Dictionary<string, string>
            {
                ["Date"] = today,
                ["TotalOrders"] = "N/A",
                ["CompletedOrders"] = "N/A",
                ["CancelledOrders"] = "N/A",
                ["TotalRevenue"] = "N/A"
            };

            await notificationService.SendNotificationAsync("DailySalesReport", adminEmail, placeholders);
            logger.LogInformation("Daily sales report sent for {Date}", today);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to generate daily sales report");
        }
    }
}
