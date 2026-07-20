using FlashShop.Inventory.Api.DTOs.Requests;
using FlashShop.Inventory.Api.Services;
using FlashShop.Shared.Common;
using FlashShop.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlashShop.Inventory.Api.Controllers;

[ApiController]
[Route("api/inventory")]
public class StocksController : ControllerBase
{
    private readonly IStockService _stockService;

    public StocksController(IStockService stockService)
    {
        _stockService = stockService;
    }

    [HttpGet("{productId:guid}")]
    public async Task<IActionResult> GetStock(Guid productId)
    {
        var result = await _stockService.GetStockAsync(productId);
        return Ok(ApiResponse<object>.SuccessResponse(result));
    }

    [HttpPost("initialize")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> InitializeStock([FromBody] InitializeStockRequest request)
    {
        var result = await _stockService.InitializeStockAsync(request);
        return CreatedAtAction(nameof(GetStock), new { productId = result.ProductId },
            ApiResponse<object>.SuccessResponse(result, "Stock initialized successfully."));
    }

    [HttpPut("{productId:guid}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> UpdateStock(Guid productId, [FromBody] UpdateStockRequest request)
    {
        var result = await _stockService.UpdateStockAsync(productId, request.Quantity);
        return Ok(ApiResponse<object>.SuccessResponse(result, "Stock updated successfully."));
    }

    [HttpGet("low-stock")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> GetLowStockItems()
    {
        var result = await _stockService.GetLowStockItemsAsync();
        return Ok(ApiResponse<object>.SuccessResponse(result));
    }
}
