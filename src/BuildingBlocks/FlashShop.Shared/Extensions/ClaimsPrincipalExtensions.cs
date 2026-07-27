using System.Security.Claims;

namespace FlashShop.Shared.Extensions;

/// <summary>
/// Provides a consistent way for services to read identity claims from a
/// validated JWT, regardless of whether standard or short claim names are used.
/// </summary>
public static class ClaimsPrincipalExtensions
{
    /// <summary>Returns the authenticated user's identifier, or an empty GUID when absent.</summary>
    public static Guid GetUserId(this ClaimsPrincipal principal)
    {
        var claim = principal.FindFirst(ClaimTypes.NameIdentifier)
            ?? principal.FindFirst("sub")
            ?? principal.FindFirst("id")
            ?? principal.FindFirst(ClaimTypes.Name);

        if (claim == null || !Guid.TryParse(claim.Value, out var userId))
        {
            return Guid.Empty;
        }
        return userId;
    }

    /// <summary>Returns the authenticated user's email claim.</summary>
    public static string GetEmail(this ClaimsPrincipal principal)
    {
        return principal.FindFirst(ClaimTypes.Email)?.Value
            ?? principal.FindFirst("email")?.Value
            ?? string.Empty;
    }

    /// <summary>Returns the role used by role-based authorization policies.</summary>
    public static string GetRole(this ClaimsPrincipal principal)
    {
        return principal.FindFirst(ClaimTypes.Role)?.Value
            ?? principal.FindFirst("role")?.Value
            ?? string.Empty;
    }
}
