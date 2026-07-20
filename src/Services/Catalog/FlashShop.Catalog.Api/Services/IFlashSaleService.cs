using FlashShop.Catalog.Api.DTOs.Requests;
using FlashShop.Catalog.Api.DTOs.Responses;

namespace FlashShop.Catalog.Api.Services;

public interface IFlashSaleService
{
    Task<List<FlashSaleCampaignResponse>> GetActiveCampaignsAsync();
    Task<FlashSaleCampaignResponse> GetCampaignByIdAsync(Guid id);
    Task<FlashSaleCampaignResponse> CreateCampaignAsync(CreateFlashSaleRequest request);
    Task<FlashSaleCampaignResponse> UpdateCampaignStatusAsync(Guid id, string status);
}
