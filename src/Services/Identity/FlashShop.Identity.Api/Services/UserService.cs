using FlashShop.Identity.Api.Data;
using FlashShop.Identity.Api.DTOs.Requests;
using FlashShop.Identity.Api.DTOs.Responses;
using FlashShop.Identity.Api.Entities;
using FlashShop.Shared.Common;
using FlashShop.Shared.Constants;
using FlashShop.Shared.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FlashShop.Identity.Api.Services;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IdentityDbContext _dbContext;

    public UserService(UserManager<ApplicationUser> userManager, IdentityDbContext dbContext)
    {
        _userManager = userManager;
        _dbContext = dbContext;
    }

    public async Task<UserResponse> GetByIdAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString())
            ?? throw new NotFoundException("User", userId);
        return await MapToResponse(user);
    }

    public async Task<UserResponse> UpdateProfileAsync(Guid userId, UpdateProfileRequest request)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString())
            ?? throw new NotFoundException("User", userId);

        if (request.FullName != null) user.FullName = request.FullName;
        if (request.PhoneNumber != null) user.PhoneNumber = request.PhoneNumber;
        if (request.AvatarUrl != null) user.AvatarUrl = request.AvatarUrl;

        await _userManager.UpdateAsync(user);
        return await MapToResponse(user);
    }

    public async Task<PagedResult<UserResponse>> GetAllUsersAsync(int page, int pageSize)
    {
        var query = _userManager.Users.AsNoTracking();
        var totalCount = await query.CountAsync();
        var users = await query
            .OrderByDescending(u => u.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var items = new List<UserResponse>();
        foreach (var user in users)
            items.Add(await MapToResponse(user));

        return new PagedResult<UserResponse>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<UserResponse> ToggleActiveAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString())
            ?? throw new NotFoundException("User", userId);
        user.IsActive = !user.IsActive;
        await _userManager.UpdateAsync(user);
        return await MapToResponse(user);
    }

    public async Task<UserResponse> ChangeRoleAsync(Guid userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString())
            ?? throw new NotFoundException("User", userId);

        var currentRoles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, currentRoles);
        await _userManager.AddToRoleAsync(user, role);
        return await MapToResponse(user);
    }

    private async Task<UserResponse> MapToResponse(ApplicationUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var wallet = await _dbContext.Wallets.AsNoTracking().FirstOrDefaultAsync(w => w.UserId == user.Id);

        return new UserResponse
        {
            Id = user.Id,
            UserName = user.UserName!,
            Email = user.Email!,
            FullName = user.FullName,
            PhoneNumber = user.PhoneNumber,
            AvatarUrl = user.AvatarUrl,
            IsActive = user.IsActive,
            Role = roles.FirstOrDefault() ?? Roles.Customer,
            WalletBalance = wallet?.Balance ?? 0m,
            CreatedAt = user.CreatedAt
        };
    }
}
