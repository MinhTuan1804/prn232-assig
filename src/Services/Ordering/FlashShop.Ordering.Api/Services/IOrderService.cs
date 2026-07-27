using FlashShop.Ordering.Api.DTOs.Requests;
using FlashShop.Ordering.Api.DTOs.Responses;
using FlashShop.Shared.Common;

namespace FlashShop.Ordering.Api.Services;

/// <summary>
/// Coordinates the order lifecycle across catalog snapshots, inventory
/// reservations, wallet payment, and integration events.
/// </summary>
public interface IOrderService
{
    /// <summary>Creates a pending order from the authenticated user's cart.</summary>
    Task<OrderResponse> CheckoutAsync(Guid userId, string userEmail, CheckoutRequest request);

    /// <summary>Returns only orders belonging to the authenticated user.</summary>
    Task<PagedResult<OrderResponse>> GetMyOrdersAsync(Guid userId, int page, int pageSize);

    /// <summary>Gets one order, optionally enforcing ownership.</summary>
    Task<OrderResponse> GetOrderByIdAsync(Guid orderId, Guid? userId = null);

    /// <summary>Charges the wallet and advances an eligible order to paid.</summary>
    Task<OrderResponse> PayOrderAsync(Guid orderId, Guid userId);

    /// <summary>Cancels an eligible order and starts compensation.</summary>
    Task<OrderResponse> CancelOrderAsync(Guid orderId, Guid userId, string? reason);

    /// <summary>Applies an administrator-controlled order status transition.</summary>
    Task<OrderResponse> UpdateStatusAsync(Guid orderId, string status);

    /// <summary>Returns a filtered, paged order list for administration.</summary>
    Task<PagedResult<OrderResponse>> GetAllOrdersAsync(int page, int pageSize, string? status);

    /// <summary>Cancels pending orders whose payment window has elapsed.</summary>
    Task CancelExpiredOrdersAsync();
}
