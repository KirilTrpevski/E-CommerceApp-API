using ECommerceShop.Data;
using ECommerceShop.Entities;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerceShop.Controllers;

[ApiController]
[Route("api/[controller]")]
[EnableCors("AllowAllOrigins")]
public class UsersController(DataContext context, UserService userService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
    {
        var users = await context.Users.ToListAsync();

        return users;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AppUser>> GetUser(Guid id)
    {
        var user = await context.Users.FindAsync(id);
        if (user == null) return NotFound();
        return Ok(user);
    }

    [HttpPut("{id}")]
    [EnableCors("AllowAllOrigins")]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUser updateUserDto)
    {
        if (id == Guid.Empty || updateUserDto == null)
        {
            return BadRequest("Invalid user data.");
        }

        // Call the service method to update the user
        var updatedUser = await userService.UpdateUserAsync(id, updateUserDto);
        if (updatedUser == null)
        {
            return NotFound("User not found.");
        }

        return Ok(updatedUser); // Return the updated user
    }
}