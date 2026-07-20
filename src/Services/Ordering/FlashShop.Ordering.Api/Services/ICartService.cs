using FlashShop.Ordering.Api.DTOs.Requests;
using FlashShop.Ordering.Api.DTOs.Responses;

namespace FlashShop.Ordering.Api.Services;

public interface ICartService
{
    Task<CartResponse> GetCartAsync(Guid userId);
    Task<CartItemResponse> AddToCartAsync(Guid userId, AddToCartRequest request);
    Task<CartItemResponse> UpdateQuantityAsync(Guid userId, Guid itemId, int quantity);
    Task RemoveItemAsync(Guid userId, Guid itemId);
    Task ClearCartAsync(Guid userId);
}
