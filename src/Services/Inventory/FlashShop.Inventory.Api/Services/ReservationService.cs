using FlashShop.Inventory.Api.Data;
using FlashShop.Inventory.Api.Entities;
using FlashShop.MessageContracts.Events;
using FlashShop.Shared.Constants;
using Microsoft.EntityFrameworkCore;

namespace FlashShop.Inventory.Api.Services;

public class ReservationService : IReservationService
{
    private readonly InventoryDbContext _context;
    private readonly ILogger<ReservationService> _logger;
    private const int MaxRetries = 3;

    public ReservationService(InventoryDbContext context, ILogger<ReservationService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> ReserveStockAsync(Guid orderId, List<OrderItemDetail> items)
    {
        for (int attempt = 0; attempt < MaxRetries; attempt++)
        {
            try
            {
                await using var transaction = await _context.Database
                    .BeginTransactionAsync(System.Data.IsolationLevel.Serializable);

                foreach (var item in items)
                {
                    var stock = await _context.Stocks.FindAsync(item.ProductId);
                    if (stock is null || stock.AvailableQuantity < item.Quantity)
                    {
                        await transaction.RollbackAsync();
                        _logger.LogWarning(
                            "Insufficient stock for product {ProductId}. Requested: {Requested}, Available: {Available}",
                            item.ProductId, item.Quantity, stock?.AvailableQuantity ?? 0);
                        return false;
                    }

                    stock.AvailableQuantity -= item.Quantity;
                    stock.ReservedQuantity += item.Quantity;
                    stock.UpdatedAt = DateTime.UtcNow;

                    _context.StockReservations.Add(new StockReservation
                    {
                        Id = Guid.NewGuid(),
                        ProductId = item.ProductId,
                        OrderId = orderId,
                        Quantity = item.Quantity,
                        Status = ReservationStatus.Reserved,
                        ExpiresAt = DateTime.UtcNow.AddMinutes(15),
                        CreatedAt = DateTime.UtcNow
                    });

                    _context.StockHistory.Add(new StockHistory
                    {
                        ProductId = item.ProductId,
                        ChangeType = "Reserve",
                        QuantityChange = -item.Quantity,
                        ReferenceId = orderId.ToString(),
                        CreatedAt = DateTime.UtcNow
                    });
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogWarning(ex,
                    "Concurrency conflict reserving stock for order {OrderId}, attempt {Attempt}/{MaxRetries}",
                    orderId, attempt + 1, MaxRetries);

                // Detach all tracked entities so next retry gets fresh data
                foreach (var entry in _context.ChangeTracker.Entries().ToList())
                {
                    entry.State = EntityState.Detached;
                }

                if (attempt == MaxRetries - 1)
                {
                    _logger.LogError("Failed to reserve stock for order {OrderId} after {MaxRetries} attempts", orderId, MaxRetries);
                    return false;
                }
            }
        }

        return false;
    }

    public async Task ConfirmReservationAsync(Guid orderId)
    {
        var reservations = await _context.StockReservations
            .Where(r => r.OrderId == orderId && r.Status == ReservationStatus.Reserved)
            .ToListAsync();

        foreach (var reservation in reservations)
        {
            reservation.Status = ReservationStatus.Confirmed;

            var stock = await _context.Stocks.FindAsync(reservation.ProductId);
            if (stock is not null)
            {
                stock.ReservedQuantity -= reservation.Quantity;
                stock.UpdatedAt = DateTime.UtcNow;
            }

            _context.StockHistory.Add(new StockHistory
            {
                ProductId = reservation.ProductId,
                ChangeType = "Confirm",
                QuantityChange = -reservation.Quantity,
                ReferenceId = orderId.ToString(),
                CreatedAt = DateTime.UtcNow
            });
        }

        await _context.SaveChangesAsync();
    }

    public async Task ReleaseReservationAsync(Guid orderId)
    {
        var reservations = await _context.StockReservations
            .Where(r => r.OrderId == orderId && r.Status == ReservationStatus.Reserved)
            .ToListAsync();

        foreach (var reservation in reservations)
        {
            reservation.Status = ReservationStatus.Released;

            var stock = await _context.Stocks.FindAsync(reservation.ProductId);
            if (stock is not null)
            {
                stock.ReservedQuantity -= reservation.Quantity;
                stock.AvailableQuantity += reservation.Quantity;
                stock.UpdatedAt = DateTime.UtcNow;
            }

            _context.StockHistory.Add(new StockHistory
            {
                ProductId = reservation.ProductId,
                ChangeType = "Release",
                QuantityChange = reservation.Quantity,
                ReferenceId = orderId.ToString(),
                CreatedAt = DateTime.UtcNow
            });
        }

        await _context.SaveChangesAsync();
    }
}
