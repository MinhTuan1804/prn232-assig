using FlashShop.Ordering.Api.Data;
using FlashShop.Ordering.Api.DTOs.Requests;
using FlashShop.Ordering.Api.DTOs.Responses;
using FlashShop.Ordering.Api.Entities;
using FlashShop.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace FlashShop.Ordering.Api.Services;

public class CartService : ICartService
{
    private readonly OrderingDbContext _dbContext;

    public CartService(OrderingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CartResponse> GetCartAsync(Guid userId)
    {
        var items = await _dbContext.CartItems
            .Where(c => c.UserId == userId)
            .OrderByDescending(c => c.AddedAt)
            .Select(c => new CartItemResponse
            {
                Id = c.Id,
                ProductId = c.ProductId,
                ProductName = c.ProductName,
                UnitPrice = c.UnitPrice,
                Quantity = c.Quantity,
                ImageUrl = c.ImageUrl,
                AddedAt = c.AddedAt
            })
            .ToListAsync();

        return new CartResponse { Items = items };
    }

    public async Task<CartItemResponse> AddToCartAsync(Guid userId, AddToCartRequest request)
    {
        if (!Guid.TryParse(request.ProductId, out var productGuid))
        {
            var hashBytes = System.Security.Cryptography.MD5.HashData(System.Text.Encoding.UTF8.GetBytes(request.ProductId));
            productGuid = new Guid(hashBytes);
        }

        var existing = await _dbContext.CartItems
            .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productGuid);

        if (existing != null)
        {
            existing.Quantity += request.Quantity;
            existing.UnitPrice = request.UnitPrice;
            await _dbContext.SaveChangesAsync();
            return MapToResponse(existing);
        }

        var cartItem = new CartItem
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ProductId = productGuid,
            ProductName = request.ProductName,
            UnitPrice = request.UnitPrice,
            Quantity = request.Quantity,
            ImageUrl = request.ImageUrl,
            AddedAt = DateTime.UtcNow
        };

        _dbContext.CartItems.Add(cartItem);
        await _dbContext.SaveChangesAsync();
        return MapToResponse(cartItem);
    }

    public async Task<CartItemResponse> UpdateQuantityAsync(Guid userId, Guid itemId, int quantity)
    {
        var item = await _dbContext.CartItems
            .FirstOrDefaultAsync(c => c.Id == itemId && c.UserId == userId)
            ?? throw new NotFoundException("CartItem", itemId);

        item.Quantity = quantity;
        await _dbContext.SaveChangesAsync();
        return MapToResponse(item);
    }

    public async Task RemoveItemAsync(Guid userId, Guid itemId)
    {
        var item = await _dbContext.CartItems
            .FirstOrDefaultAsync(c => c.Id == itemId && c.UserId == userId)
            ?? throw new NotFoundException("CartItem", itemId);

        _dbContext.CartItems.Remove(item);
        await _dbContext.SaveChangesAsync();
    }

    public async Task ClearCartAsync(Guid userId)
    {
        var items = await _dbContext.CartItems.Where(c => c.UserId == userId).ToListAsync();
        _dbContext.CartItems.RemoveRange(items);
        await _dbContext.SaveChangesAsync();
    }

    private static CartItemResponse MapToResponse(CartItem item) => new()
    {
        Id = item.Id,
        ProductId = item.ProductId,
        ProductName = item.ProductName,
        UnitPrice = item.UnitPrice,
        Quantity = item.Quantity,
        ImageUrl = item.ImageUrl,
        AddedAt = item.AddedAt
    };
}
