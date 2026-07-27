using FlashShop.Identity.Api.DTOs.Requests;
using FlashShop.Identity.Api.DTOs.Responses;
using FlashShop.Shared.Common;

namespace FlashShop.Identity.Api.Services;

/// <summary>Manages user profiles and administrator-controlled account state.</summary>
public interface IUserService
{
    /// <summary>Gets one user projection by its persistent identifier.</summary>
    Task<UserResponse> GetByIdAsync(Guid userId);

    /// <summary>Updates editable profile fields for the authenticated user.</summary>
    Task<UserResponse> UpdateProfileAsync(Guid userId, UpdateProfileRequest request);

    /// <summary>Returns a paged user list for administration.</summary>
    Task<PagedResult<UserResponse>> GetAllUsersAsync(int page, int pageSize);

    /// <summary>Enables or disables an account without deleting its history.</summary>
    Task<UserResponse> ToggleActiveAsync(Guid userId);

    /// <summary>Changes the role used by role-based authorization.</summary>
    Task<UserResponse> ChangeRoleAsync(Guid userId, string role);
}
