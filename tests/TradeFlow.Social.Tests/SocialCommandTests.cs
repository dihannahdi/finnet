using FluentAssertions;
using MassTransit;
using Moq;
using TradeFlow.Social.Application.Commands;
using TradeFlow.Social.Domain.Entities;
using TradeFlow.Social.Domain.Interfaces;
using Xunit;

namespace TradeFlow.Social.Tests;

public class SocialCommandTests
{
    [Fact]
    public async Task CreateTradeIdea_ShouldSucceed()
    {
        // Arrange
        var repo = new Mock<ITradeIdeaRepository>();
        var publishEndpoint = new Mock<IPublishEndpoint>();
        repo.Setup(x => x.AddAsync(It.IsAny<TradeIdea>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TradeIdea idea, CancellationToken _) => idea);

        var handler = new CreateTradeIdeaCommandHandler(repo.Object, publishEndpoint.Object);
        var command = new CreateTradeIdeaCommand
        {
            AuthorId = Guid.NewGuid(),
            AuthorName = "Test User",
            Symbol = "AAPL",
            Direction = "Bullish",
            Title = "Apple is going up",
            Content = "Based on earnings..."
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Symbol.Should().Be("AAPL");
        result.Value.Direction.Should().Be("Bullish");
    }

    [Fact]
    public async Task FollowUser_ShouldSucceed_WhenNotAlreadyFollowing()
    {
        // Arrange
        var followRepo = new Mock<IFollowRepository>();
        var notifRepo = new Mock<INotificationRepository>();
        followRepo.Setup(x => x.GetFollowAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Follow?)null);
        followRepo.Setup(x => x.AddAsync(It.IsAny<Follow>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Follow f, CancellationToken _) => f);
        notifRepo.Setup(x => x.AddAsync(It.IsAny<Notification>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Notification n, CancellationToken _) => n);

        var handler = new FollowUserCommandHandler(followRepo.Object, notifRepo.Object);
        var command = new FollowUserCommand { FollowerId = Guid.NewGuid(), FollowingId = Guid.NewGuid() };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task FollowUser_ShouldFail_WhenAlreadyFollowing()
    {
        // Arrange
        var followRepo = new Mock<IFollowRepository>();
        var notifRepo = new Mock<INotificationRepository>();
        var existingFollow = Follow.Create(Guid.NewGuid(), Guid.NewGuid());
        followRepo.Setup(x => x.GetFollowAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingFollow);

        var handler = new FollowUserCommandHandler(followRepo.Object, notifRepo.Object);
        var command = new FollowUserCommand { FollowerId = Guid.NewGuid(), FollowingId = Guid.NewGuid() };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Already following");
    }
}
