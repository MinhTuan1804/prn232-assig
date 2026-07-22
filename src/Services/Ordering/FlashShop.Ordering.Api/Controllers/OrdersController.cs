using FlashShop.Ordering.Api.DTOs.Requests;
using FlashShop.Ordering.Api.Services;
using FlashShop.Shared.Common;
using FlashShop.Shared.Constants;
using FlashShop.Shared.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlashShop.Ordering.Api.Controllers;

[ApiController]
[Route("api/orders")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout([FromBody] CheckoutRequest request)
    {
        var userId = User.GetUserId();
        var email = User.GetEmail();
        var result = await _orderService.CheckoutAsync(userId, email, request);
        return Ok(ApiResponse<object>.SuccessResponse(result, "Order created successfully."));
    }

    [HttpGet("my-orders")]
    public async Task<IActionResult> GetMyOrders([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var userId = User.GetUserId();
        var result = await _orderService.GetMyOrdersAsync(userId, page, pageSize);
        return Ok(ApiResponse<object>.SuccessResponse(result));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var userId = User.GetUserId();
        var role = User.GetRole();
        var result = await _orderService.GetOrderByIdAsync(id, role == Roles.Admin ? null : userId);
        return Ok(ApiResponse<object>.SuccessResponse(result));
    }

    [HttpPost("{id:guid}/pay")]
    public async Task<IActionResult> Pay(Guid id)
    {
        var userId = User.GetUserId();
        var result = await _orderService.PayOrderAsync(id, userId);
        return Ok(ApiResponse<object>.SuccessResponse(result, "Payment processed successfully."));
    }

    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id, [FromBody] CancelOrderRequest? request)
    {
        var userId = User.GetUserId();
        var result = await _orderService.CancelOrderAsync(id, userId, request?.Reason);
        return Ok(ApiResponse<object>.SuccessResponse(result, "Order cancelled."));
    }

    [HttpPut("{id:guid}/status")]
    [Authorize]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateOrderStatusRequest request)
    {
        var result = await _orderService.UpdateStatusAsync(id, request.Status);
        return Ok(ApiResponse<object>.SuccessResponse(result, "Order status updated."));
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? status = null)
    {
        var result = await _orderService.GetAllOrdersAsync(page, pageSize, status);
        return Ok(ApiResponse<object>.SuccessResponse(result));
    }
}

public class CancelOrderRequest
{
    public string? Reason { get; set; }
}
