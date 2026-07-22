using FlashShop.Catalog.Api.Data;
using FlashShop.Catalog.Api.DTOs.Requests;
using FlashShop.Catalog.Api.DTOs.Responses;
using FlashShop.Catalog.Api.Entities;
using FlashShop.Shared.Constants;
using FlashShop.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace FlashShop.Catalog.Api.Services;

public class FlashSaleService : IFlashSaleService
{
    private readonly CatalogDbContext _context;

    public FlashSaleService(CatalogDbContext context)
    {
        _context = context;
    }

    public async Task<List<FlashSaleCampaignResponse>> GetActiveCampaignsAsync()
    {
        var campaigns = await _context.FlashSaleCampaigns
            .Include(c => c.Items)
                .ThenInclude(i => i.Product)
            .OrderByDescending(c => c.StartTime)
            .ToListAsync();

        return campaigns.Select(MapToCampaignResponse).ToList();
    }

    public async Task<List<FlashSaleCampaignResponse>> GetAllCampaignsAsync()
    {
        var campaigns = await _context.FlashSaleCampaigns
            .Include(c => c.Items)
                .ThenInclude(i => i.Product)
            .OrderByDescending(c => c.StartTime)
            .ToListAsync();

        return campaigns.Select(MapToCampaignResponse).ToList();
    }

    public async Task<FlashSaleCampaignResponse> GetCampaignByIdAsync(Guid id)
    {
        var campaign = await _context.FlashSaleCampaigns
            .Include(c => c.Items)
                .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (campaign is null)
            throw new NotFoundException(nameof(FlashSaleCampaign), id);

        return MapToCampaignResponse(campaign);
    }

    public async Task<FlashSaleCampaignResponse> CreateCampaignAsync(CreateFlashSaleRequest request)
    {
        if (request.EndTime <= request.StartTime)
            throw new BadRequestException("EndTime must be after StartTime.");

        // Validate all product IDs exist
        var productIds = request.Items.Select(i => i.ProductId).Distinct().ToList();
        var existingProducts = await _context.Products
            .Where(p => productIds.Contains(p.Id))
            .Select(p => p.Id)
            .ToListAsync();

        var missingProducts = productIds.Except(existingProducts).ToList();
        if (missingProducts.Any())
            throw new NotFoundException($"Products not found: {string.Join(", ", missingProducts)}");

        var campaign = new FlashSaleCampaign
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            Status = CampaignStatus.Scheduled
        };

        foreach (var item in request.Items)
        {
            campaign.Items.Add(new FlashSaleItem
            {
                ProductId = item.ProductId,
                FlashSalePrice = item.FlashSalePrice,
                FlashSaleQuantity = item.FlashSaleQuantity,
                MaxPerUser = item.MaxPerUser
            });
        }

        _context.FlashSaleCampaigns.Add(campaign);
        await _context.SaveChangesAsync();

        // Reload with navigation properties
        var created = await _context.FlashSaleCampaigns
            .Include(c => c.Items)
                .ThenInclude(i => i.Product)
            .FirstAsync(c => c.Id == campaign.Id);

        return MapToCampaignResponse(created);
    }

    public async Task<FlashSaleCampaignResponse> UpdateCampaignStatusAsync(Guid id, string status)
    {
        var validStatuses = new[] { CampaignStatus.Scheduled, CampaignStatus.Active, CampaignStatus.Ended };
        if (!validStatuses.Contains(status))
            throw new BadRequestException($"Invalid status. Valid values: {string.Join(", ", validStatuses)}");

        var campaign = await _context.FlashSaleCampaigns
            .Include(c => c.Items)
                .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (campaign is null)
            throw new NotFoundException(nameof(FlashSaleCampaign), id);

        campaign.Status = status;
        await _context.SaveChangesAsync();

        return MapToCampaignResponse(campaign);
    }

    private static FlashSaleCampaignResponse MapToCampaignResponse(FlashSaleCampaign campaign)
    {
        return new FlashSaleCampaignResponse
        {
            Id = campaign.Id,
            Name = campaign.Name,
            StartTime = campaign.StartTime,
            EndTime = campaign.EndTime,
            Status = campaign.Status,
            Items = campaign.Items.Select(i => new FlashSaleItemResponse
            {
                ProductId = i.ProductId,
                ProductName = i.Product?.Name ?? string.Empty,
                ProductImageUrl = i.Product?.ImageUrl,
                OriginalPrice = i.Product?.Price ?? 0,
                FlashSalePrice = i.FlashSalePrice,
                FlashSaleQuantity = i.FlashSaleQuantity,
                SoldQuantity = i.SoldQuantity,
                StockQuantity = i.Product?.StockQuantity ?? Math.Max(0, i.FlashSaleQuantity - i.SoldQuantity),
                MaxPerUser = i.MaxPerUser
            }).ToList()
        };
    }
}
