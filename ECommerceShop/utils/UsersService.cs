using ECommerceShop.Data;
using ECommerceShop.Entities;

public class UserService
{
    private readonly DataContext _context;

    public UserService(DataContext context)
    {
        _context = context;
    }

    // Update user information
    public async Task<AppUser> UpdateUserAsync(Guid userId, UpdateUser updateUserDto)
    {
        // Find the user by ID
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return null; // User not found
        }

        // Update user properties
        user.UserName = updateUserDto.UserName;
        user.Email = updateUserDto.Email;
        user.IsAdmin = updateUserDto.IsAdmin;

        // Save changes to the database
        _context.Users.Update(user);
        await _context.SaveChangesAsync();

        return user; // Return updated user
    }
}