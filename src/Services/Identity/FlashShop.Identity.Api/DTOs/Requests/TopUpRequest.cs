using System.ComponentModel.DataAnnotations;

namespace FlashShop.Identity.Api.DTOs.Requests;

public class TopUpRequest
{
    public Guid? UserId { get; set; }

    [Required, Range(1000, 1000000000)]
    public decimal Amount { get; set; }
}
