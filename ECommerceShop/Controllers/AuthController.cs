using ECommerceShop.Data;
using ECommerceShop.Entities;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

[Route("api/[controller]")]
[ApiController]
[EnableCors("AllowAllOrigins")]
public class AuthController(DataContext dbContext, PasswordService passwordService, IConfiguration configuration) : ControllerBase
{
    private readonly string _jwtSecretKey = configuration["Jwt:SecretKey"];

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginUser model)
    {
        // Check if the user exists in the database
        var user = dbContext.Users.SingleOrDefault(u => u.UserName == model.Username);
        if (user == null)
        {
            return Unauthorized("Invalid username or password.");
        }

        // Verify the password using the password service
        if (!passwordService.VerifyPassword(model.Password, user.Password))
        {
            return Unauthorized("Invalid username or password.");
        }

        // Generate JWT token if credentials are valid
        var token = GenerateJwtToken(user);
        var userId = user.Id;

        return Ok(new { token, userId });
    }

    [HttpPost("signup")]
    public async Task<IActionResult> Signup([FromBody] SignUpUser model)
    {
        // Validate input
        if (model == null || string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
        {
            return BadRequest("Username, email, and password are required.");
        }

        // Check if username or email already exists
        var existingUser = dbContext.Users.SingleOrDefault(u => u.UserName == model.Username);
        if (existingUser != null)
        {
            return Conflict("Username already taken.");
        }

        var existingEmail = dbContext.Users.SingleOrDefault(u => u.Email == model.Email);
        if (existingEmail != null)
        {
            return Conflict("Email already in use.");
        }

        // Hash the password
        var hashedPassword = passwordService.HashPassword(model.Password);

        // Create a new user
        var newUser = new AppUser
        {
            Id = Guid.NewGuid(),
            UserName = model.Username,
            Email = model.Email,
            Password = hashedPassword,
            IsAdmin = model.IsAdmin
        };

        // Save user to the database
        dbContext.Users.Add(newUser);
        await dbContext.SaveChangesAsync();

        return Ok("User registered successfully.");
    }

    private string GenerateJwtToken(AppUser user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim( "IsAdmin", user.IsAdmin.ToString())  // You can add roles if needed
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.Now.AddHours(1);

        var token = new JwtSecurityToken(
            issuer: "https://localhost:5001",  // Issuer URL
            audience: "https://localhost:5001",  // Audience URL
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
