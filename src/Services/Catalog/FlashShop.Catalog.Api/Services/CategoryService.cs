using FlashShop.Catalog.Api.Data;
using FlashShop.Catalog.Api.DTOs.Requests;
using FlashShop.Catalog.Api.DTOs.Responses;
using FlashShop.Catalog.Api.Entities;
using FlashShop.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace FlashShop.Catalog.Api.Services;

public class CategoryService : ICategoryService
{
    private readonly CatalogDbContext _context;

    public CategoryService(CatalogDbContext context)
    {
        _context = context;
    }

    public async Task<List<CategoryResponse>> GetAllAsync()
    {
        return await _context.Categories
            .OrderBy(c => c.SortOrder)
            .Select(c => new CategoryResponse
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                IsActive = c.IsActive,
                SortOrder = c.SortOrder,
                ProductCount = c.Products.Count
            })
            .ToListAsync();
    }

    public async Task<CategoryResponse> GetByIdAsync(int id)
    {
        var category = await _context.Categories
            .Where(c => c.Id == id)
            .Select(c => new CategoryResponse
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                IsActive = c.IsActive,
                SortOrder = c.SortOrder,
                ProductCount = c.Products.Count
            })
            .FirstOrDefaultAsync();

        if (category is null)
            throw new NotFoundException(nameof(Category), id);

        return category;
    }

    public async Task<CategoryResponse> CreateAsync(CreateCategoryRequest request)
    {
        var exists = await _context.Categories.AnyAsync(c => c.Name == request.Name);
        if (exists)
            throw new ConflictException($"Category with name '{request.Name}' already exists.");

        var category = new Category
        {
            Name = request.Name,
            Description = request.Description
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        return new CategoryResponse
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            IsActive = category.IsActive,
            SortOrder = category.SortOrder,
            ProductCount = 0
        };
    }

    public async Task<CategoryResponse> UpdateAsync(int id, UpdateCategoryRequest request)
    {
        var category = await _context.Categories
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category is null)
            throw new NotFoundException(nameof(Category), id);

        if (request.Name is not null)
        {
            var exists = await _context.Categories.AnyAsync(c => c.Name == request.Name && c.Id != id);
            if (exists)
                throw new ConflictException($"Category with name '{request.Name}' already exists.");

            category.Name = request.Name;
        }

        if (request.Description is not null)
            category.Description = request.Description;

        await _context.SaveChangesAsync();

        return new CategoryResponse
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            IsActive = category.IsActive,
            SortOrder = category.SortOrder,
            ProductCount = category.Products.Count
        };
    }

    public async Task DeleteAsync(int id)
    {
        var category = await _context.Categories
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category is null)
            throw new NotFoundException(nameof(Category), id);

        if (category.Products.Any())
            throw new BadRequestException("Cannot delete category that has products.");

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
    }
}
