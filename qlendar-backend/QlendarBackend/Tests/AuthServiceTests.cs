using Xunit;
using FluentAssertions;
using Moq;
using QlendarBackend.Qlendar.API;
using QlendarBackend.Qlendar.Models;
using Microsoft.AspNetCore.Identity;

namespace Qlendar.Tests.Unit.Services;

public class AuthServiceTests
{
    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly AuthService _authService;
    private readonly IConfiguration _configuration;

    public AuthServiceTests()
    {
        _userManagerMock = new Mock<UserManager<User>>(
            Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                ["Jwt:Key"] = "SuperSecretKeyWithMinimum128Bits",
                ["Jwt:Issuer"] = "Qlendar",
                ["Jwt:Audience"] = "QlendarUsers",
                ["Jwt:ExpireMinutes"] = "60"
            })
            .Build();

        _authService = new AuthService(_userManagerMock.Object, _configuration);
    }

    [Fact]
    public async Task RegisterAsync_ShouldReturnSuccess_WhenNewUser()
    {
        // Arrange
        _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((User)null);

        _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _authService.RegisterAsync("test@example.com", "Password123!");

        // Assert
        result.Success.Should().BeTrue();
        result.Token.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnToken_WhenCredentialsValid()
    {
        // Arrange
        var testUser = new User { Email = "test@example.com" };
        _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(testUser);

        _userManagerMock.Setup(x => x.CheckPasswordAsync(testUser, It.IsAny<string>()))
            .ReturnsAsync(true);

        // Act
        var result = await _authService.LoginAsync("test@example.com", "Password123!");

        // Assert
        result.Success.Should().BeTrue();
        result.Token.Should().NotBeNullOrEmpty();
    }
}