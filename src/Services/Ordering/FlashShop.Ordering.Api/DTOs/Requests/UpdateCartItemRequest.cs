using System.ComponentModel.DataAnnotations;

namespace FlashShop.Ordering.Api.DTOs.Requests;

public class UpdateCartItemRequest
{
    [Required, Range(1, 100)]
    public int Quantity { get; set; }
}
