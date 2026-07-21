using FlashShop.Catalog.Api.DTOs.Requests;
using FlashShop.Catalog.Api.DTOs.Responses;
using FlashShop.Shared.Common;

namespace FlashShop.Catalog.Api.Services;

public interface IProductService
{
    Task<PagedResult<ProductResponse>> GetPagedAsync(ProductQueryParams queryParams);
    Task<List<ProductResponse>> GetHotDealsAsync();
    Task<ProductResponse> GetByIdAsync(Guid id);
    Task<ProductResponse> CreateAsync(CreateProductRequest request);
    Task<ProductResponse> UpdateAsync(Guid id, UpdateProductRequest request);
    Task DeleteAsync(Guid id);
}
