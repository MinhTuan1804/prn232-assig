using System.ComponentModel.DataAnnotations;

namespace FlashShop.Inventory.Api.DTOs.Requests;

public record UpdateStockRequest
{
    [Range(0, int.MaxValue)]
    public int Quantity { get; init; }
}
