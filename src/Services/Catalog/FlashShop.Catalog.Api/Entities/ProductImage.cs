namespace FlashShop.Catalog.Api.Entities;

public class ProductImage
{
    public int Id { get; set; }
    public Guid ProductId { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public int SortOrder { get; set; }

    public Product Product { get; set; } = null!;
}
