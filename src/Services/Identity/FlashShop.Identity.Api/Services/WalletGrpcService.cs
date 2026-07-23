using FlashShop.Identity.Api.DTOs.Requests;
using FlashShop.MessageContracts.Protos;
using Grpc.Core;

namespace FlashShop.Identity.Api.Services;

public class WalletGrpcService : WalletGrpc.WalletGrpcBase
{
    private readonly IWalletService _walletService;
    private readonly ILogger<WalletGrpcService> _logger;

    public WalletGrpcService(IWalletService walletService, ILogger<WalletGrpcService> logger)
    {
        _walletService = walletService;
        _logger = logger;
    }

    public override async Task<FlashShop.MessageContracts.Protos.WalletPaymentResponse> PayWithWallet(FlashShop.MessageContracts.Protos.WalletPaymentRequest request, ServerCallContext context)
    {
        _logger.LogInformation("gRPC PayWithWallet request received for UserId: {UserId}, Amount: {Amount}", request.UserId, request.Amount);

        try
        {
            if (!Guid.TryParse(request.UserId, out var userId))
            {
                return new FlashShop.MessageContracts.Protos.WalletPaymentResponse
                {
                    IsSuccess = false,
                    Message = "Invalid User ID format."
                };
            }

            var walletReq = new FlashShop.Identity.Api.DTOs.Requests.WalletPaymentRequest
            {
                UserId = userId,
                Amount = (decimal)request.Amount,
                OrderId = request.OrderId,
                Description = request.Description
            };

            var walletRes = await _walletService.ProcessPaymentAsync(walletReq);

            return new FlashShop.MessageContracts.Protos.WalletPaymentResponse
            {
                IsSuccess = true,
                Message = "Payment successful via gRPC.",
                RemainingBalance = (double)walletRes.Balance
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "gRPC PayWithWallet failed for UserId: {UserId}", request.UserId);
            return new FlashShop.MessageContracts.Protos.WalletPaymentResponse
            {
                IsSuccess = false,
                Message = ex.Message
            };
        }
    }

    public override async Task<FlashShop.MessageContracts.Protos.GetWalletBalanceResponse> GetWalletBalance(FlashShop.MessageContracts.Protos.GetWalletBalanceRequest request, ServerCallContext context)
    {
        try
        {
            if (!Guid.TryParse(request.UserId, out var userId))
            {
                return new FlashShop.MessageContracts.Protos.GetWalletBalanceResponse
                {
                    IsSuccess = false,
                    Message = "Invalid User ID format."
                };
            }

            var walletRes = await _walletService.GetBalanceAsync(userId);
            return new FlashShop.MessageContracts.Protos.GetWalletBalanceResponse
            {
                IsSuccess = true,
                Message = "Success",
                Balance = (double)walletRes.Balance
            };
        }
        catch (Exception ex)
        {
            return new FlashShop.MessageContracts.Protos.GetWalletBalanceResponse
            {
                IsSuccess = false,
                Message = ex.Message
            };
        }
    }
}
