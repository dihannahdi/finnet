using FluentAssertions;
using Moq;
using TradeFlow.Identity.Application.Commands;
using TradeFlow.Identity.Domain.Entities;
using TradeFlow.Identity.Domain.Interfaces;
using Xunit;

namespace TradeFlow.Identity.Tests;

public class AuthCommandTests
{
    private readonly Mock<IUserRepository> _userRepo = new();
    private readonly Mock<ITokenService> _tokenService = new();
    private readonly Mock<IPasswordHasher> _passwordHasher = new();

    [Fact]
    public async Task Register_ShouldSucceed_WhenEmailNotTaken()
    {
        // Arrange
        _userRepo.Setup(x => x.ExistsByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _passwordHasher.Setup(x => x.Hash(It.IsAny<string>())).Returns("hashed");
        _tokenService.Setup(x => x.GenerateAccessToken(It.IsAny<User>())).Returns("jwt-token");
        _tokenService.Setup(x => x.GenerateRefreshToken()).Returns("refresh-token");

        var handler = new RegisterCommandHandler(_userRepo.Object, _tokenService.Object, _passwordHasher.Object);
        var command = new RegisterCommand { Email = "test@test.com", Password = "Password123!", DisplayName = "Test User" };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.AccessToken.Should().Be("jwt-token");
        _userRepo.Verify(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Register_ShouldFail_WhenEmailAlreadyExists()
    {
        // Arrange
        _userRepo.Setup(x => x.ExistsByEmailAsync("test@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var handler = new RegisterCommandHandler(_userRepo.Object, _tokenService.Object, _passwordHasher.Object);
        var command = new RegisterCommand { Email = "test@test.com", Password = "Password123!", DisplayName = "Test User" };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("already exists");
    }

    [Fact]
    public async Task Login_ShouldSucceed_WithCorrectCredentials()
    {
        // Arrange
        var user = User.Create("test@test.com", "Test User", "hashed");
        _userRepo.Setup(x => x.GetByEmailAsync("test@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _passwordHasher.Setup(x => x.Verify("Password123!", "hashed")).Returns(true);
        _tokenService.Setup(x => x.GenerateAccessToken(It.IsAny<User>())).Returns("jwt-token");
        _tokenService.Setup(x => x.GenerateRefreshToken()).Returns("refresh-token");

        var handler = new LoginCommandHandler(_userRepo.Object, _tokenService.Object, _passwordHasher.Object);
        var command = new LoginCommand { Email = "test@test.com", Password = "Password123!" };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.AccessToken.Should().Be("jwt-token");
    }

    [Fact]
    public async Task Login_ShouldFail_WithWrongPassword()
    {
        // Arrange
        var user = User.Create("test@test.com", "Test User", "hashed");
        _userRepo.Setup(x => x.GetByEmailAsync("test@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _passwordHasher.Setup(x => x.Verify("wrong", "hashed")).Returns(false);

        var handler = new LoginCommandHandler(_userRepo.Object, _tokenService.Object, _passwordHasher.Object);
        var command = new LoginCommand { Email = "test@test.com", Password = "wrong" };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
    }
}
