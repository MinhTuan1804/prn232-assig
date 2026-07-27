using FlashShop.Ordering.Api.DTOs.Requests;
using FlashShop.Ordering.Api.DTOs.Responses;

namespace FlashShop.Ordering.Api.Services;

/// <summary>Maintains the authenticated user's mutable pre-checkout basket.</summary>
public interface ICartService
{
    /// <summary>Gets the current cart and calculated line totals.</summary>
    Task<CartResponse> GetCartAsync(Guid userId);

    /// <summary>Adds a product or increases its existing cart quantity.</summary>
    Task<CartItemResponse> AddToCartAsync(Guid userId, AddToCartRequest request);

    /// <summary>Sets the requested quantity for a cart line owned by the user.</summary>
    Task<CartItemResponse> UpdateQuantityAsync(Guid userId, Guid itemId, int quantity);

    /// <summary>Removes one line from the user's cart.</summary>
    Task RemoveItemAsync(Guid userId, Guid itemId);

    /// <summary>Removes all cart lines after checkout or explicit clearing.</summary>
    Task ClearCartAsync(Guid userId);
}
