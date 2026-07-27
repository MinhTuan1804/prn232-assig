using FlashShop.Catalog.Api.DTOs.Requests;
using FlashShop.Catalog.Api.DTOs.Responses;

namespace FlashShop.Catalog.Api.Services;

/// <summary>Maintains the category hierarchy used to organize products.</summary>
public interface ICategoryService
{
    /// <summary>Lists all available categories.</summary>
    Task<List<CategoryResponse>> GetAllAsync();

    /// <summary>Gets a category by its numeric identifier.</summary>
    Task<CategoryResponse> GetByIdAsync(int id);

    /// <summary>Creates a category from administrator input.</summary>
    Task<CategoryResponse> CreateAsync(CreateCategoryRequest request);

    /// <summary>Updates an existing category.</summary>
    Task<CategoryResponse> UpdateAsync(int id, UpdateCategoryRequest request);

    /// <summary>Removes a category when business rules allow it.</summary>
    Task DeleteAsync(int id);
}
