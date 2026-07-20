using System.ComponentModel.DataAnnotations;

namespace FlashShop.Ordering.Api.DTOs.Requests;

public class UpdateOrderStatusRequest
{
    [Required]
    public string Status { get; set; } = string.Empty;
}
