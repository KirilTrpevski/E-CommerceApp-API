using ECommerceShop.Controllers;
using ECommerceShop.Data;
using ECommerceShop.Entities;
using ECommerceShop;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ECommerceShop.Tests.Controllers
{
    public class AuthControllerTests
    {
        private readonly DbContextOptions<DataContext> _dbContextOptions;
        private readonly Mock<PasswordService> _passwordServiceMock;
        private readonly IConfiguration _configuration;

        public AuthControllerTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Fresh DB for each test
                .Options;

            _passwordServiceMock = new Mock<PasswordService>();

            var configData = new Dictionary<string, string>
            {
                {"Jwt:SecretKey", "ThisIsASecretKeyForTestingPurposes"}
            };
            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configData)
                .Build();
        }

        [Fact]
        public async Task Signup_WithValidData_ShouldReturnOk()
        {
            // Arrange
            using var context = new DataContext(_dbContextOptions);
            var controller = new AuthController(context, _passwordServiceMock.Object, _configuration);

            var signUpUser = new SignUpUser
            {
                Username = "testuser",
                Email = "testuser@example.com",
                Password = "Password123!",
                IsAdmin = false
            };

            _passwordServiceMock.Setup(ps => ps.HashPassword(It.IsAny<string>())).Returns("hashedpassword");

            // Act
            var result = await controller.Signup(signUpUser);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("User registered successfully.", okResult.Value);
        }

        [Fact]
        public void Login_WithInvalidUsername_ShouldReturnUnauthorized()
        {
            // Arrange
            using var context = new DataContext(_dbContextOptions);
            var controller = new AuthController(context, _passwordServiceMock.Object, _configuration);

            var loginUser = new LoginUser
            {
                Username = "nonexistentuser",
                Password = "Password123!"
            };

            // Act
            var result = controller.Login(loginUser);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Invalid username or password.", unauthorizedResult.Value);
        }

        [Fact]
        public void Login_WithIncorrectPassword_ShouldReturnUnauthorized()
        {
            // Arrange
            using var context = new DataContext(_dbContextOptions);
            context.Users.Add(new AppUser
            {
                Id = Guid.NewGuid(),
                UserName = "testuser",
                Email = "testuser@example.com",
                Password = "hashedpassword", // Stored hashed password
                IsAdmin = false,
                ProductInteractions = new List<ProductInteraction>()
            });
            context.SaveChanges();

            var controller = new AuthController(context, _passwordServiceMock.Object, _configuration);

            var loginUser = new LoginUser
            {
                Username = "testuser",
                Password = "WrongPassword!"
            };

            _passwordServiceMock.Setup(ps => ps.VerifyPassword(It.IsAny<string>(), It.IsAny<string>())).Returns(false);

            // Act
            var result = controller.Login(loginUser);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Invalid username or password.", unauthorizedResult.Value);
        }

        [Fact]
        public void Login_WithValidCredentials_ShouldReturnOk()
        {
            // Arrange
            using var context = new DataContext(_dbContextOptions);
            var user = new AppUser
            {
                Id = Guid.NewGuid(),
                UserName = "validuser",
                Email = "validuser@example.com",
                Password = "hashedpassword",
                IsAdmin = true,
                ProductInteractions = new List<ProductInteraction>()
            };
            context.Users.Add(user);
            context.SaveChanges();

            var controller = new AuthController(context, _passwordServiceMock.Object, _configuration);

            var loginUser = new LoginUser
            {
                Username = "validuser",
                Password = "Password123!"
            };

            _passwordServiceMock.Setup(ps => ps.VerifyPassword(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            // Act
            var result = controller.Login(loginUser);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = okResult.Value as dynamic;

            Assert.NotNull(response);
            Assert.NotNull(response.token);
            Assert.Equal(user.Id, (Guid)response.userId);
        }
    }
}
