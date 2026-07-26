using FlashShop.Catalog.Api.Data;
using FlashShop.MessageContracts.Protos;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;

namespace FlashShop.Catalog.Api.Services;

public class CatalogGrpcService : CatalogGrpc.CatalogGrpcBase
{
    private readonly CatalogDbContext _dbContext;
    private readonly ILogger<CatalogGrpcService> _logger;

    public CatalogGrpcService(CatalogDbContext dbContext, ILogger<CatalogGrpcService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public override async Task<DeductStockResponse> DeductStock(DeductStockRequest request, ServerCallContext context)
    {
        _logger.LogInformation("gRPC DeductStock request received for {Count} items", request.Items.Count);

        try
        {
            foreach (var item in request.Items)
            {
                if (!Guid.TryParse(item.ProductId, out var productId))
                {
                    _logger.LogWarning("Invalid product ID format in gRPC DeductStock: {ProductId}", item.ProductId);
                    continue;
                }

                var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == productId);
                if (product != null)
                {
                    product.StockQuantity = Math.Max(0, product.StockQuantity - item.Quantity);
                    product.UpdatedAt = DateTime.UtcNow;
                }

                var flashSaleItem = await _dbContext.FlashSaleItems.FirstOrDefaultAsync(f => f.ProductId == productId);
                if (flashSaleItem != null)
                {
                    flashSaleItem.SoldQuantity += item.Quantity;
                }
            }

            await _dbContext.SaveChangesAsync();

            return new DeductStockResponse
            {
                IsSuccess = true,
                Message = "Stock updated successfully via gRPC."
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing gRPC DeductStock");
            return new DeductStockResponse
            {
                IsSuccess = false,
                Message = ex.Message
            };
        }
    }
}
