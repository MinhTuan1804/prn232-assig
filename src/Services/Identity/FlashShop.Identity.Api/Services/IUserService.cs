using FlashShop.Identity.Api.DTOs.Requests;
using FlashShop.Identity.Api.DTOs.Responses;
using FlashShop.Shared.Common;

namespace FlashShop.Identity.Api.Services;

public interface IUserService
{
    Task<UserResponse> GetByIdAsync(Guid userId);
    Task<UserResponse> UpdateProfileAsync(Guid userId, UpdateProfileRequest request);
    Task<PagedResult<UserResponse>> GetAllUsersAsync(int page, int pageSize);
    Task<UserResponse> ToggleActiveAsync(Guid userId);
    Task<UserResponse> ChangeRoleAsync(Guid userId, string role);
}
