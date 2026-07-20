using FlashShop.Inventory.Api.DTOs.Requests;
using FlashShop.Inventory.Api.DTOs.Responses;

namespace FlashShop.Inventory.Api.Services;

public interface IStockService
{
    Task<StockResponse> GetStockAsync(Guid productId);
    Task<StockResponse> InitializeStockAsync(InitializeStockRequest request);
    Task<StockResponse> UpdateStockAsync(Guid productId, int quantity);
    Task<List<StockResponse>> GetLowStockItemsAsync();
}
