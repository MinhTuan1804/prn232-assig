using System.ComponentModel.DataAnnotations;

namespace FlashShop.Identity.Api.DTOs.Requests;

public class RegisterRequest
{
    [Required, MaxLength(256)]
    public string UserName { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(256)]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(6)]
    public string Password { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string FullName { get; set; } = string.Empty;

    [Phone]
    public string? PhoneNumber { get; set; }
}
