using FlashShop.Ordering.Api.DTOs.Requests;
using FlashShop.Ordering.Api.DTOs.Responses;
using FlashShop.Shared.Common;

namespace FlashShop.Ordering.Api.Services;

public interface IOrderService
{
    Task<OrderResponse> CheckoutAsync(Guid userId, string userEmail, CheckoutRequest request);
    Task<PagedResult<OrderResponse>> GetMyOrdersAsync(Guid userId, int page, int pageSize);
    Task<OrderResponse> GetOrderByIdAsync(Guid orderId, Guid? userId = null);
    Task<OrderResponse> PayOrderAsync(Guid orderId, Guid userId);
    Task<OrderResponse> CancelOrderAsync(Guid orderId, Guid userId, string? reason);
    Task<OrderResponse> UpdateStatusAsync(Guid orderId, string status);
    Task<PagedResult<OrderResponse>> GetAllOrdersAsync(int page, int pageSize, string? status);
    Task CancelExpiredOrdersAsync();
}
