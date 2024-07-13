using BookLibrary.Business.AuthenticateAggregate;
using BookLibrary.Business.Services.PasswordHasher;
using BookLibrary.Data.Entities;
using BookLibrary.Infrastructure.AppSettings;
using BookLibrary.Models;
using BookLibrary.Repositories.Abstractions;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using System.Linq.Expressions;
using System.Net;

namespace BookLibrary.Tests
{
    public class AuthenticationServiceTests
    {
        private readonly Mock<IAuthenticationRepository> _mockRepo;
        private readonly IOptions<AppSettings> _appSettings;
        private readonly IOptions<AuthenticationSettings> _authSettings;
        private readonly AuthenticationService _service;

        public AuthenticationServiceTests()
        {
            _mockRepo = new Mock<IAuthenticationRepository>();
            _appSettings = Options.Create(new AppSettings { TokenValidity = 30, RegistrationCodeValidity = 60 });
            _authSettings = Options.Create(new AuthenticationSettings
            {
                JwtBearer = new JwtBearerSettings
                {
                    SecurityKey = "A8A12445FBC3D8E9A5F2B745D86F1C8B5C84A24E91D7A3C5C7D9A2B3146F5E78",
                    Issuer = "test-issuer",
                    Audience = "test-audience"
                }
            });

            _service = new AuthenticationService(_mockRepo.Object, _appSettings, _authSettings);
        }

        [Fact]
        public void AuthenticateUser_WithValidCredentials_ReturnsSuccessfulResponse()
        {
            // Arrange
            var request = new AuthenticateRequest { Username = "test@example.com", Password = "password123" };
            var user = new User
            {
                UserId = 1,
                Email = "test@example.com",
                Password = PasswordHash.CreateHash("password123"),
                IsActive = true,
                Role = new Role { Name = "User" }
            };
            _mockRepo.Setup(r => r.Get(
                It.IsAny<Expression<Func<User, bool>>>(),
                It.IsAny<Func<IQueryable<User>, IOrderedQueryable<User>>>(),
                It.IsAny<Expression<Func<User, object>>>()))
            .Returns(new List<User> { user }.AsQueryable());

            // Act
            var result = _service.AuthenticateUser(request);

            // Assert
            result?.Should().NotBeNull();
            result?.Success.Should().BeTrue();
            result?.StatusCode.Should().Be((int)HttpStatusCode.OK);
            result?.Message.Should().Be("User authenticated successfully");
            result?.Data.Should().NotBeNull();
            result?.Data?.Email.Should().Be("test@example.com");

            // Verify that the repository method was called
            _mockRepo.Verify(r => r.Get(
                It.IsAny<Expression<Func<User, bool>>>(),
                It.IsAny<Func<IQueryable<User>, IOrderedQueryable<User>>>(),
                It.IsAny<Expression<Func<User, object>>>()),
                Times.Once);
        }

        [Fact]
        public void AuthenticateUser_WithInvalidCredentials_ReturnsUnauthorizedResponse()
        {
            // Arrange
            var request = new AuthenticateRequest { Username = "test@example.com", Password = "wrongpassword" };
            var user = new User
            {
                UserId = 1,
                Email = "test@example.com",
                Password = PasswordHash.CreateHash("correctpassword"),
                IsActive = true,
                Role = new Role { Name = "User" }
            };
            _mockRepo.Setup(r => r.Get(
                It.IsAny<Expression<Func<User, bool>>>(),
                It.IsAny<Func<IQueryable<User>, IOrderedQueryable<User>>>(),
                It.IsAny<Expression<Func<User, object>>>()))
                    .Returns(new List<User> { user }.AsQueryable());

            // Act
            var result = _service.AuthenticateUser(request);

            // Assert
            result.Should().NotBeNull();
            result?.Success.Should().BeFalse();
            result?.StatusCode.Should().Be((int)HttpStatusCode.Unauthorized);
            result?.Message.Should().Be("Unauthorized User");
            result?.Data.Should().BeNull();
        }
        


        [Fact]
        public async Task RegisterUser_WithExistingEmail_ReturnsErrorResponse()
        {
            // Arrange
            var email = "existing@example.com";
            var existingUser = new User
            {
                UserId = 1,
                Email = email,
                Password = "somepassword",
                Role = new Role { Name = "User" }
            };

            _mockRepo.Setup(r => r.Get(
                It.IsAny<Expression<Func<User, bool>>>(),
                It.IsAny<Func<IQueryable<User>, IOrderedQueryable<User>>>(),
                It.IsAny<Expression<Func<User, object>>>()))
            .Returns(new List<User> { existingUser }.AsQueryable());

            _mockRepo.Setup(r => r.GetRole(It.IsAny<string>()))
                .Returns(new Role { RoleId = 1, Name = "User" });

            // Act
            var result = await _service.RegisterUser(email);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.Message.Should().Be("Username/Email already exists.");
            result.Data.Should().BeNull();
        }
    }
}