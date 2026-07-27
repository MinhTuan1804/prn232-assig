using FlashShop.Catalog.Api.DTOs.Requests;
using FlashShop.Catalog.Api.DTOs.Responses;
using FlashShop.Shared.Common;

namespace FlashShop.Catalog.Api.Services;

/// <summary>Provides product discovery and administrator product maintenance.</summary>
public interface IProductService
{
    /// <summary>Searches and filters products with pagination.</summary>
    Task<PagedResult<ProductResponse>> GetPagedAsync(ProductQueryParams queryParams);

    /// <summary>Lists products currently promoted as hot deals.</summary>
    Task<List<ProductResponse>> GetHotDealsAsync();

    /// <summary>Gets the full public projection of one product.</summary>
    Task<ProductResponse> GetByIdAsync(Guid id);

    /// <summary>Creates a product in the catalog.</summary>
    Task<ProductResponse> CreateAsync(CreateProductRequest request);

    /// <summary>Updates catalog-owned product data.</summary>
    Task<ProductResponse> UpdateAsync(Guid id, UpdateProductRequest request);

    /// <summary>Deletes a product when it is no longer offered.</summary>
    Task DeleteAsync(Guid id);
}
