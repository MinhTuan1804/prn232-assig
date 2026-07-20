using System.ComponentModel.DataAnnotations;

namespace FlashShop.Identity.Api.DTOs.Requests;

public class TopUpRequest
{
    [Required, Range(1000, 100000000)]
    public decimal Amount { get; set; }
}
