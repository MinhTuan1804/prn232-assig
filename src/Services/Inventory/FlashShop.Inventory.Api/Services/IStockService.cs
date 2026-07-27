using FlashShop.Inventory.Api.DTOs.Requests;
using FlashShop.Inventory.Api.DTOs.Responses;

namespace FlashShop.Inventory.Api.Services;

/// <summary>Owns available product quantities and low-stock monitoring.</summary>
public interface IStockService
{
    /// <summary>Gets the current stock projection for a catalog product.</summary>
    Task<StockResponse> GetStockAsync(Guid productId);

    /// <summary>Creates the first stock record for a product.</summary>
    Task<StockResponse> InitializeStockAsync(InitializeStockRequest request);

    /// <summary>Applies an administrator quantity adjustment.</summary>
    Task<StockResponse> UpdateStockAsync(Guid productId, int quantity);

    /// <summary>Lists products at or below their configured threshold.</summary>
    Task<List<StockResponse>> GetLowStockItemsAsync();
}
