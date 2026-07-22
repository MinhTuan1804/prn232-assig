using System.Security.Claims;

namespace FlashShop.Shared.Extensions;

public static class ClaimsPrincipalExtensions
{
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

    public static string GetEmail(this ClaimsPrincipal principal)
    {
        return principal.FindFirst(ClaimTypes.Email)?.Value
            ?? principal.FindFirst("email")?.Value
            ?? string.Empty;
    }

    public static string GetRole(this ClaimsPrincipal principal)
    {
        return principal.FindFirst(ClaimTypes.Role)?.Value
            ?? principal.FindFirst("role")?.Value
            ?? string.Empty;
    }
}
