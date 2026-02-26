using FluentAssertions;
using TradeFlow.Social.Domain.Entities;
using Xunit;

namespace TradeFlow.Social.Tests;

public class TradeIdeaDomainTests
{
    [Fact]
    public void Create_ShouldInitializeCorrectly()
    {
        var idea = TradeIdea.Create(Guid.NewGuid(), "Test User", "AAPL", "Bullish", "Title", "Content");

        idea.Symbol.Should().Be("AAPL");
        idea.Direction.Should().Be("Bullish");
        idea.LikesCount.Should().Be(0);
        idea.CommentsCount.Should().Be(0);
        idea.IsActive.Should().BeTrue();
    }

    [Fact]
    public void AddLike_ShouldIncrementCount()
    {
        var idea = TradeIdea.Create(Guid.NewGuid(), "Test User", "AAPL", "Bullish", "Title", "Content");
        idea.AddLike(Guid.NewGuid());

        idea.LikesCount.Should().Be(1);
        idea.Likes.Should().HaveCount(1);
    }

    [Fact]
    public void AddLike_ShouldNotDuplicate()
    {
        var userId = Guid.NewGuid();
        var idea = TradeIdea.Create(Guid.NewGuid(), "Test User", "AAPL", "Bullish", "Title", "Content");
        idea.AddLike(userId);
        idea.AddLike(userId);

        idea.LikesCount.Should().Be(1);
    }

    [Fact]
    public void RemoveLike_ShouldDecrementCount()
    {
        var userId = Guid.NewGuid();
        var idea = TradeIdea.Create(Guid.NewGuid(), "Test User", "AAPL", "Bullish", "Title", "Content");
        idea.AddLike(userId);
        idea.RemoveLike(userId);

        idea.LikesCount.Should().Be(0);
        idea.Likes.Should().BeEmpty();
    }

    [Fact]
    public void AddComment_ShouldIncrementCount()
    {
        var idea = TradeIdea.Create(Guid.NewGuid(), "Test User", "AAPL", "Bullish", "Title", "Content");
        idea.AddComment(Guid.NewGuid(), "Commenter", "Great analysis!");

        idea.CommentsCount.Should().Be(1);
        idea.Comments.Should().HaveCount(1);
    }

    [Fact]
    public void Follow_Create_ShouldThrow_WhenFollowingSelf()
    {
        var userId = Guid.NewGuid();
        var act = () => Follow.Create(userId, userId);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Cannot follow yourself*");
    }
}
