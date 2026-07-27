using FlashShop.Identity.Api.DTOs.Requests;
using FlashShop.Identity.Api.DTOs.Responses;

namespace FlashShop.Identity.Api.Services;

/// <summary>Coordinates account registration and JWT issuance.</summary>
public interface IAuthService
{
    /// <summary>Creates a customer account and returns its initial authentication result.</summary>
    Task<AuthResponse> RegisterAsync(RegisterRequest request);

    /// <summary>Validates credentials and issues a JWT for the matching user.</summary>
    Task<AuthResponse> LoginAsync(LoginRequest request);

    /// <summary>Issues a replacement token for an existing active user.</summary>
    Task<AuthResponse> RefreshTokenAsync(Guid userId);
}
