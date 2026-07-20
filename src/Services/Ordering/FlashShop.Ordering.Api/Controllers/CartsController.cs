using FlashShop.Ordering.Api.DTOs.Requests;
using FlashShop.Ordering.Api.Services;
using FlashShop.Shared.Common;
using FlashShop.Shared.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlashShop.Ordering.Api.Controllers;

[ApiController]
[Route("api/cart")]
[Authorize]
public class CartsController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartsController(ICartService cartService)
    {
        _cartService = cartService;
    }

    [HttpGet]
    public async Task<IActionResult> GetCart()
    {
        var userId = User.GetUserId();
        var result = await _cartService.GetCartAsync(userId);
        return Ok(ApiResponse<object>.SuccessResponse(result));
    }

    [HttpPost("items")]
    public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest request)
    {
        var userId = User.GetUserId();
        var result = await _cartService.AddToCartAsync(userId, request);
        return Ok(ApiResponse<object>.SuccessResponse(result, "Item added to cart."));
    }

    [HttpPut("items/{itemId:guid}")]
    public async Task<IActionResult> UpdateQuantity(Guid itemId, [FromBody] UpdateCartItemRequest request)
    {
        var userId = User.GetUserId();
        var result = await _cartService.UpdateQuantityAsync(userId, itemId, request.Quantity);
        return Ok(ApiResponse<object>.SuccessResponse(result));
    }

    [HttpDelete("items/{itemId:guid}")]
    public async Task<IActionResult> RemoveItem(Guid itemId)
    {
        var userId = User.GetUserId();
        await _cartService.RemoveItemAsync(userId, itemId);
        return Ok(ApiResponse<object>.SuccessResponse(null!, "Item removed from cart."));
    }

    [HttpDelete]
    public async Task<IActionResult> ClearCart()
    {
        var userId = User.GetUserId();
        await _cartService.ClearCartAsync(userId);
        return Ok(ApiResponse<object>.SuccessResponse(null!, "Cart cleared."));
    }
}
