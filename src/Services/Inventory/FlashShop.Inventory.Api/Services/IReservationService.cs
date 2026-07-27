using FlashShop.MessageContracts.Events;

namespace FlashShop.Inventory.Api.Services;

/// <summary>
/// Coordinates temporary stock reservations for the asynchronous order saga.
/// Reservations prevent concurrent orders from spending the same units.
/// </summary>
public interface IReservationService
{
    /// <summary>Attempts to reserve every requested item for an order.</summary>
    Task<bool> ReserveStockAsync(Guid orderId, List<OrderItemDetail> items);

    /// <summary>Finalizes a reservation after payment succeeds.</summary>
    Task ConfirmReservationAsync(Guid orderId);

    /// <summary>Returns reserved units after cancellation or payment timeout.</summary>
    Task ReleaseReservationAsync(Guid orderId);
}
