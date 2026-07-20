using System.ComponentModel.DataAnnotations;

namespace FlashShop.Identity.Api.DTOs.Requests;

public class UpdateProfileRequest
{
    [MaxLength(200)]
    public string? FullName { get; set; }

    [Phone]
    public string? PhoneNumber { get; set; }

    [MaxLength(500)]
    public string? AvatarUrl { get; set; }
}
