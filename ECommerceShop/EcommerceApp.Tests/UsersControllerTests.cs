using ECommerceShop.Controllers;
using ECommerceShop.Data;
using ECommerceShop.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace ECommerceShop.Tests.Controllers
{
    public class UsersControllerTests
    {
        private readonly UsersController _controller;
        private readonly DataContext _context;
        private readonly UserService _userService;

        public UsersControllerTests()
        {
            // Create an in-memory database for testing
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new DataContext(options); // Using the in-memory database
            _userService = new UserService(_context); // Assuming UserService takes DataContext as dependency
            _controller = new UsersController(_context, _userService);
        }

        [Fact]
        public async Task GetUser_InvalidId_ShouldReturnNotFound()
        {
            // Act
            var result = await _controller.GetUser(Guid.NewGuid());

            // Assert
            var actionResult = Assert.IsType<ActionResult<AppUser>>(result); // Make sure it's of type ActionResult<AppUser>
            Assert.IsType<NotFoundResult>(actionResult.Result); // Check that the result is of type NotFoundResult
        }


        [Fact]
        public async Task UpdateUser_ValidData_ShouldReturnOkWithUpdatedUser()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new AppUser
            {
                Id = userId,
                UserName = "OldUser",
                Email = "olduser@example.com",
                Password = "password1",
                IsAdmin = false
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var updateUserDto = new UpdateUser
            {
                UserName = "UpdatedUser",
                Email = "updateduser@example.com",
                IsAdmin = true
            };

            // Act
            var result = await _controller.UpdateUser(userId, updateUserDto);

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result);
            var updatedUser = Assert.IsType<AppUser>(actionResult.Value);
            Assert.Equal("UpdatedUser", updatedUser.UserName);
            Assert.Equal("updateduser@example.com", updatedUser.Email);
            Assert.True(updatedUser.IsAdmin);
        }

        [Fact]
        public async Task UpdateUser_InvalidData_ShouldReturnBadRequest()
        {
            // Act
            var result = await _controller.UpdateUser(Guid.Empty, null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid user data.", badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateUser_UserNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var updateUserDto = new UpdateUser
            {
                UserName = "UpdatedUser",
                Email = "updateduser@example.com",
                IsAdmin = true
            };

            // Act
            var result = await _controller.UpdateUser(Guid.NewGuid(), updateUserDto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("User not found.", notFoundResult.Value);
        }
    }
}
