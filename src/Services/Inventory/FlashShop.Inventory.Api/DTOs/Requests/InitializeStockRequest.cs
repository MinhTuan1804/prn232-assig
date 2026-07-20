using System.ComponentModel.DataAnnotations;

namespace FlashShop.Inventory.Api.DTOs.Requests;

public record InitializeStockRequest
{
    public Guid ProductId { get; init; }

    [Range(0, int.MaxValue)]
    public int Quantity { get; init; }
}
