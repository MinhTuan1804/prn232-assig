using FlashShop.Catalog.Api.DTOs.Requests;
using FlashShop.Catalog.Api.DTOs.Responses;

namespace FlashShop.Catalog.Api.Services;

/// <summary>Manages time-bound campaigns and their public active state.</summary>
public interface IFlashSaleService
{
    /// <summary>Returns campaigns that are currently available to customers.</summary>
    Task<List<FlashSaleCampaignResponse>> GetActiveCampaignsAsync();

    /// <summary>Returns all campaigns for administration.</summary>
    Task<List<FlashSaleCampaignResponse>> GetAllCampaignsAsync();

    /// <summary>Gets one campaign and its configured products.</summary>
    Task<FlashSaleCampaignResponse> GetCampaignByIdAsync(Guid id);

    /// <summary>Creates a campaign and its sale window.</summary>
    Task<FlashSaleCampaignResponse> CreateCampaignAsync(CreateFlashSaleRequest request);

    /// <summary>Moves a campaign to a supported lifecycle status.</summary>
    Task<FlashSaleCampaignResponse> UpdateCampaignStatusAsync(Guid id, string status);
}
