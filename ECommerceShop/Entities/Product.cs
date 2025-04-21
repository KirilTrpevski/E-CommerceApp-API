namespace ECommerceShop.Entities;
public class Product
{
    public required Guid Id { get; set; }
    public required decimal Price { get; set; }
    public required string Description { get; set; }
    public required string Name { get; set; }
    public required string Category { get; set; }

    public required int Stock { get; set; }

    public byte[] Image { get; set; }

    public ICollection<ProductInteraction> ProductInteractions { get; set; }
}
