namespace ECommerceShop.Entities;

public class SignUpUser
{
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public bool IsAdmin { get; set; } = false;
}