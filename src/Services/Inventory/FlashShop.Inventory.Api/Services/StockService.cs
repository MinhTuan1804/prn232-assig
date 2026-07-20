using FlashShop.Inventory.Api.Data;
using FlashShop.Inventory.Api.DTOs.Requests;
using FlashShop.Inventory.Api.DTOs.Responses;
using FlashShop.Inventory.Api.Entities;
using FlashShop.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace FlashShop.Inventory.Api.Services;

public class StockService : IStockService
{
    private readonly InventoryDbContext _context;

    public StockService(InventoryDbContext context)
    {
        _context = context;
    }

    public async Task<StockResponse> GetStockAsync(Guid productId)
    {
        var stock = await _context.Stocks.FindAsync(productId)
            ?? throw new NotFoundException("Stock", productId);

        return MapToResponse(stock);
    }

    public async Task<StockResponse> InitializeStockAsync(InitializeStockRequest request)
    {
        var existing = await _context.Stocks.FindAsync(request.ProductId);
        if (existing is not null)
            throw new ConflictException($"Stock for product '{request.ProductId}' already exists.");

        var stock = new Stock
        {
            ProductId = request.ProductId,
            AvailableQuantity = request.Quantity,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Stocks.Add(stock);
        await _context.SaveChangesAsync();

        return MapToResponse(stock);
    }

    public async Task<StockResponse> UpdateStockAsync(Guid productId, int quantity)
    {
        var stock = await _context.Stocks.FindAsync(productId)
            ?? throw new NotFoundException("Stock", productId);

        stock.AvailableQuantity = quantity;
        stock.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return MapToResponse(stock);
    }

    public async Task<List<StockResponse>> GetLowStockItemsAsync()
    {
        var items = await _context.Stocks
            .Where(s => s.AvailableQuantity <= s.MinThreshold)
            .ToListAsync();

        return items.Select(MapToResponse).ToList();
    }

    private static StockResponse MapToResponse(Stock stock) => new()
    {
        ProductId = stock.ProductId,
        AvailableQuantity = stock.AvailableQuantity,
        ReservedQuantity = stock.ReservedQuantity,
        MinThreshold = stock.MinThreshold,
        UpdatedAt = stock.UpdatedAt
    };
}
