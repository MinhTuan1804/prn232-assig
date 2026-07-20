using FlashShop.MessageContracts.Events;

namespace FlashShop.Inventory.Api.Services;

public interface IReservationService
{
    Task<bool> ReserveStockAsync(Guid orderId, List<OrderItemDetail> items);
    Task ConfirmReservationAsync(Guid orderId);
    Task ReleaseReservationAsync(Guid orderId);
}
