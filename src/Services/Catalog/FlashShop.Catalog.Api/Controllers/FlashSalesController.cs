using FlashShop.Catalog.Api.DTOs.Requests;
using FlashShop.Catalog.Api.Services;
using FlashShop.Shared.Common;
using FlashShop.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlashShop.Catalog.Api.Controllers;

[ApiController]
[Route("api/flashsales")]
public class FlashSalesController : ControllerBase
{
    private readonly IFlashSaleService _flashSaleService;

    public FlashSalesController(IFlashSaleService flashSaleService)
    {
        _flashSaleService = flashSaleService;
    }

    [HttpGet("active")]
    public async Task<IActionResult> GetActive()
    {
        var campaigns = await _flashSaleService.GetActiveCampaignsAsync();
        return Ok(ApiResponse<object>.SuccessResponse(campaigns));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var campaign = await _flashSaleService.GetCampaignByIdAsync(id);
        return Ok(ApiResponse<object>.SuccessResponse(campaign));
    }

    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Create([FromBody] CreateFlashSaleRequest request)
    {
        var campaign = await _flashSaleService.CreateCampaignAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = campaign.Id },
            ApiResponse<object>.SuccessResponse(campaign, "Flash sale campaign created successfully."));
    }

    [HttpPut("{id:guid}/status")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateStatusRequest request)
    {
        var campaign = await _flashSaleService.UpdateCampaignStatusAsync(id, request.Status);
        return Ok(ApiResponse<object>.SuccessResponse(campaign, "Campaign status updated successfully."));
    }
}

public class UpdateStatusRequest
{
    public string Status { get; set; } = string.Empty;
}
