namespace FlashShop.Identity.Api.DTOs.Responses;

public class AuthResponse
{
    public string Token { get; set; } = string.Empty;
    public DateTime Expiration { get; set; }
    public UserResponse User { get; set; } = null!;
}
