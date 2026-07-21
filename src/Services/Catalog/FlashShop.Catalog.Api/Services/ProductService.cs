using System.Text.RegularExpressions;
using FlashShop.Catalog.Api.Data;
using FlashShop.Catalog.Api.DTOs.Requests;
using FlashShop.Catalog.Api.DTOs.Responses;
using FlashShop.Catalog.Api.Entities;
using FlashShop.Shared.Common;
using FlashShop.Shared.Constants;
using FlashShop.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace FlashShop.Catalog.Api.Services;

public class ProductService : IProductService
{
    private readonly CatalogDbContext _context;

    public ProductService(CatalogDbContext context)
    {
        _context = context;
    }

    public async Task<List<ProductResponse>> GetHotDealsAsync()
    {
        var products = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Images)
            .Where(p => p.IsActive)
            .OrderByDescending(p => p.CreatedAt)
            .Take(12)
            .ToListAsync();

        return products.Select(MapToProductResponse).ToList();
    }

    public async Task<PagedResult<ProductResponse>> GetPagedAsync(ProductQueryParams queryParams)
    {
        var query = _context.Products
            .Include(p => p.Category)
            .Include(p => p.Images)
            .AsQueryable();

        // Filtering
        if (queryParams.CategoryId.HasValue)
            query = query.Where(p => p.CategoryId == queryParams.CategoryId.Value);

        if (!string.IsNullOrWhiteSpace(queryParams.Search))
            query = query.Where(p => p.Name.Contains(queryParams.Search));

        if (queryParams.MinPrice.HasValue)
            query = query.Where(p => p.Price >= queryParams.MinPrice.Value);

        if (queryParams.MaxPrice.HasValue)
            query = query.Where(p => p.Price <= queryParams.MaxPrice.Value);

        // Sorting
        query = queryParams.SortBy?.ToLower() switch
        {
            "price" => queryParams.SortDir?.ToLower() == "desc"
                ? query.OrderByDescending(p => p.Price)
                : query.OrderBy(p => p.Price),
            "createdat" => queryParams.SortDir?.ToLower() == "desc"
                ? query.OrderByDescending(p => p.CreatedAt)
                : query.OrderBy(p => p.CreatedAt),
            _ => queryParams.SortDir?.ToLower() == "desc"
                ? query.OrderByDescending(p => p.Name)
                : query.OrderBy(p => p.Name)
        };

        var totalCount = await query.CountAsync();

        var products = await query
            .Skip((queryParams.Page - 1) * queryParams.PageSize)
            .Take(queryParams.PageSize)
            .ToListAsync();

        var now = DateTime.UtcNow;
        var productIds = products.Select(p => p.Id).ToList();

        // Get active flash sale items for these products
        var activeFlashSaleItems = await _context.FlashSaleItems
            .Include(fi => fi.Campaign)
            .Where(fi => productIds.Contains(fi.ProductId)
                && (fi.Campaign.Status == CampaignStatus.Active
                    || (fi.Campaign.StartTime <= now && fi.Campaign.EndTime >= now)))
            .ToListAsync();

        var flashSaleMap = activeFlashSaleItems
            .GroupBy(fi => fi.ProductId)
            .ToDictionary(g => g.Key, g => g.First());

        var items = products.Select(p =>
        {
            var response = MapToProductResponse(p);
            if (flashSaleMap.TryGetValue(p.Id, out var flashItem))
            {
                response.FlashSalePrice = flashItem.FlashSalePrice;
                response.FlashSaleEndTime = flashItem.Campaign.EndTime;
            }
            return response;
        }).ToList();

        return new PagedResult<ProductResponse>
        {
            Items = items,
            TotalCount = totalCount,
            Page = queryParams.Page,
            PageSize = queryParams.PageSize
        };
    }

    public async Task<ProductResponse> GetByIdAsync(Guid id)
    {
        var product = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product is null)
            throw new NotFoundException(nameof(Product), id);

        var response = MapToProductResponse(product);

        // Check active flash sale
        var now = DateTime.UtcNow;
        var flashItem = await _context.FlashSaleItems
            .Include(fi => fi.Campaign)
            .Where(fi => fi.ProductId == id
                && (fi.Campaign.Status == CampaignStatus.Active
                    || (fi.Campaign.StartTime <= now && fi.Campaign.EndTime >= now)))
            .FirstOrDefaultAsync();

        if (flashItem is not null)
        {
            response.FlashSalePrice = flashItem.FlashSalePrice;
            response.FlashSaleEndTime = flashItem.Campaign.EndTime;
        }

        return response;
    }

    public async Task<ProductResponse> CreateAsync(CreateProductRequest request)
    {
        var categoryExists = await _context.Categories.AnyAsync(c => c.Id == request.CategoryId);
        if (!categoryExists)
            throw new NotFoundException(nameof(Category), request.CategoryId);

        var slug = GenerateSlug(request.Name);

        // Ensure slug uniqueness
        var baseSlug = slug;
        var counter = 1;
        while (await _context.Products.AnyAsync(p => p.Slug == slug))
        {
            slug = $"{baseSlug}-{counter}";
            counter++;
        }

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Slug = slug,
            Description = request.Description,
            Price = request.Price,
            ImageUrl = request.ImageUrl,
            CategoryId = request.CategoryId
        };

        // Add additional images
        if (request.ImageUrls.Any())
        {
            for (int i = 0; i < request.ImageUrls.Count; i++)
            {
                product.Images.Add(new ProductImage
                {
                    ImageUrl = request.ImageUrls[i],
                    SortOrder = i
                });
            }
        }

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        // Reload with navigation properties
        var created = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Images)
            .FirstAsync(p => p.Id == product.Id);

        return MapToProductResponse(created);
    }

    public async Task<ProductResponse> UpdateAsync(Guid id, UpdateProductRequest request)
    {
        var product = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product is null)
            throw new NotFoundException(nameof(Product), id);

        if (request.Name is not null)
        {
            product.Name = request.Name;
            var slug = GenerateSlug(request.Name);
            var baseSlug = slug;
            var counter = 1;
            while (await _context.Products.AnyAsync(p => p.Slug == slug && p.Id != id))
            {
                slug = $"{baseSlug}-{counter}";
                counter++;
            }
            product.Slug = slug;
        }

        if (request.Description is not null)
            product.Description = request.Description;

        if (request.Price.HasValue)
            product.Price = request.Price.Value;

        if (request.ImageUrl is not null)
            product.ImageUrl = request.ImageUrl;

        if (request.CategoryId.HasValue)
        {
            var categoryExists = await _context.Categories.AnyAsync(c => c.Id == request.CategoryId.Value);
            if (!categoryExists)
                throw new NotFoundException(nameof(Category), request.CategoryId.Value);

            product.CategoryId = request.CategoryId.Value;
        }

        if (request.ImageUrls is not null)
        {
            // Replace existing images
            _context.ProductImages.RemoveRange(product.Images);
            for (int i = 0; i < request.ImageUrls.Count; i++)
            {
                product.Images.Add(new ProductImage
                {
                    ImageUrl = request.ImageUrls[i],
                    SortOrder = i
                });
            }
        }

        product.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        // Reload with navigation properties
        var updated = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Images)
            .FirstAsync(p => p.Id == id);

        return MapToProductResponse(updated);
    }

    public async Task DeleteAsync(Guid id)
    {
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);

        if (product is null)
            throw new NotFoundException(nameof(Product), id);

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
    }

    private static ProductResponse MapToProductResponse(Product product)
    {
        return new ProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            Slug = product.Slug,
            Description = product.Description,
            Price = product.Price,
            ImageUrl = product.ImageUrl,
            CategoryId = product.CategoryId,
            CategoryName = product.Category?.Name ?? string.Empty,
            IsActive = product.IsActive,
            Images = product.Images.OrderBy(i => i.SortOrder).Select(i => new ProductImageResponse
            {
                Id = i.Id,
                ImageUrl = i.ImageUrl,
                SortOrder = i.SortOrder
            }).ToList(),
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };
    }

    private static string GenerateSlug(string name)
    {
        var slug = name.ToLower().Trim();
        slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");
        slug = Regex.Replace(slug, @"\s+", "-");
        slug = Regex.Replace(slug, @"-+", "-");
        slug = slug.Trim('-');
        return slug;
    }
}
