namespace ECommerceShop.Entities;

public class AppUser
{
    public Guid Id { get; set; }
    public required string UserName { get; set; }

    public required string Email { get; set; }

    public required string Password { get; set; }

    public required bool IsAdmin { get; set; } = false;

    public ICollection<ProductInteraction> ProductInteractions { get; set; }

}
