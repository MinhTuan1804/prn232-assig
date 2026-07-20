using FlashShop.Identity.Api.DTOs.Requests;
using FlashShop.Identity.Api.Services;
using FlashShop.Shared.Common;
using FlashShop.Shared.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlashShop.Identity.Api.Controllers;

[ApiController]
[Route("api/wallets")]
[Authorize]
public class WalletsController : ControllerBase
{
    private readonly IWalletService _walletService;

    public WalletsController(IWalletService walletService)
    {
        _walletService = walletService;
    }

    [HttpGet("balance")]
    public async Task<IActionResult> GetBalance()
    {
        var userId = User.GetUserId();
        var result = await _walletService.GetBalanceAsync(userId);
        return Ok(ApiResponse<object>.SuccessResponse(result));
    }

    [HttpPost("topup")]
    public async Task<IActionResult> TopUp([FromBody] TopUpRequest request)
    {
        var userId = User.GetUserId();
        var result = await _walletService.TopUpAsync(userId, request.Amount);
        return Ok(ApiResponse<object>.SuccessResponse(result, "Top up successful."));
    }

    [HttpPost("pay")]
    public async Task<IActionResult> ProcessPayment([FromBody] WalletPaymentRequest request)
    {
        var result = await _walletService.ProcessPaymentAsync(request);
        return Ok(ApiResponse<object>.SuccessResponse(result, "Payment successful."));
    }

    [HttpGet("transactions")]
    public async Task<IActionResult> GetTransactions([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var userId = User.GetUserId();
        var result = await _walletService.GetTransactionsAsync(userId, page, pageSize);
        return Ok(ApiResponse<object>.SuccessResponse(result));
    }
}
