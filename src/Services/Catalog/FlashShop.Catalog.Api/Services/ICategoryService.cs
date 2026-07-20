using FlashShop.Catalog.Api.DTOs.Requests;
using FlashShop.Catalog.Api.DTOs.Responses;

namespace FlashShop.Catalog.Api.Services;

public interface ICategoryService
{
    Task<List<CategoryResponse>> GetAllAsync();
    Task<CategoryResponse> GetByIdAsync(int id);
    Task<CategoryResponse> CreateAsync(CreateCategoryRequest request);
    Task<CategoryResponse> UpdateAsync(int id, UpdateCategoryRequest request);
    Task DeleteAsync(int id);
}
