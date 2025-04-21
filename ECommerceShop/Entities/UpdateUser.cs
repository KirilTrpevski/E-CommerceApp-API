namespace ECommerceShop.Entities;

public class UpdateUser
{
    public required string UserName { get; set; }

    public required string Email { get; set; }

    public required bool IsAdmin { get; set; } = false;

}