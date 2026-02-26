using FluentAssertions;
using TradeFlow.Social.Domain.Entities;
using Xunit;

namespace TradeFlow.Social.Tests;

public class TradeIdeaDomainTests
{
    private TradeIdea CreateTestIdea() =>
        TradeIdea.Create(Guid.NewGuid(), "Test User", "AAPL", "Bullish", "Apple Bull Case", "Strong earnings expected");

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
    public void Create_ShouldSetAllProperties()
    {
        var authorId = Guid.NewGuid();
        var idea = TradeIdea.Create(authorId, "John", "AAPL", "Bullish", "Title", "Content", 200m, 140m);

        idea.AuthorId.Should().Be(authorId);
        idea.AuthorName.Should().Be("John");
        idea.TargetPrice.Should().Be(200m);
        idea.StopLoss.Should().Be(140m);
    }

    [Fact]
    public void Create_ShouldNormalizeSymbol()
    {
        var idea = TradeIdea.Create(Guid.NewGuid(), "User", "aapl", "Bullish", "Title", "Content");
        idea.Symbol.Should().Be("AAPL");
    }

    [Fact]
    public void AddLike_ShouldIncrementCount()
    {
        var idea = CreateTestIdea();
        var userId = Guid.NewGuid();
        idea.AddLike(userId);

        idea.LikesCount.Should().Be(1);
        idea.Likes.Should().HaveCount(1);
        idea.Likes.First().UserId.Should().Be(userId);
    }

    [Fact]
    public void AddLike_ShouldNotDuplicate()
    {
        var userId = Guid.NewGuid();
        var idea = CreateTestIdea();
        idea.AddLike(userId);
        idea.AddLike(userId);

        idea.LikesCount.Should().Be(1);
    }

    [Fact]
    public void AddLike_MultipleDifferentUsers_ShouldTrackAll()
    {
        var idea = CreateTestIdea();
        idea.AddLike(Guid.NewGuid());
        idea.AddLike(Guid.NewGuid());
        idea.AddLike(Guid.NewGuid());

        idea.LikesCount.Should().Be(3);
    }

    [Fact]
    public void RemoveLike_ShouldDecrementCount()
    {
        var userId = Guid.NewGuid();
        var idea = CreateTestIdea();
        idea.AddLike(userId);
        idea.RemoveLike(userId);

        idea.LikesCount.Should().Be(0);
        idea.Likes.Should().BeEmpty();
    }

    [Fact]
    public void RemoveLike_ShouldDoNothing_WhenNotLiked()
    {
        var idea = CreateTestIdea();
        idea.RemoveLike(Guid.NewGuid());
        idea.LikesCount.Should().Be(0);
    }

    [Fact]
    public void AddComment_ShouldIncrementCount()
    {
        var idea = CreateTestIdea();
        var comment = idea.AddComment(Guid.NewGuid(), "Commenter", "Great analysis!");

        idea.CommentsCount.Should().Be(1);
        idea.Comments.Should().HaveCount(1);
        comment.Content.Should().Be("Great analysis!");
    }

    [Fact]
    public void AddComment_Multiple_ShouldTrackAll()
    {
        var idea = CreateTestIdea();
        idea.AddComment(Guid.NewGuid(), "User1", "Comment 1");
        idea.AddComment(Guid.NewGuid(), "User2", "Comment 2");

        idea.CommentsCount.Should().Be(2);
        idea.Comments.Should().HaveCount(2);
    }

    [Fact]
    public void Follow_Create_ShouldThrow_WhenFollowingSelf()
    {
        var userId = Guid.NewGuid();
        var act = () => Follow.Create(userId, userId);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Cannot follow yourself*");
    }

    [Fact]
    public void Follow_Create_ShouldSucceed_WhenDifferentUsers()
    {
        var follow = Follow.Create(Guid.NewGuid(), Guid.NewGuid());
        follow.Should().NotBeNull();
        follow.FollowerId.Should().NotBe(follow.FollowingId);
    }

    [Fact]
    public void Notification_MarkAsRead_ShouldSetFlag()
    {
        var notification = Notification.Create(Guid.NewGuid(), "Trade", "New Trade", "AAPL bought");
        notification.IsRead.Should().BeFalse();

        notification.MarkAsRead();
        notification.IsRead.Should().BeTrue();
    }

    [Fact]
    public void Notification_Create_ShouldSetProperties()
    {
        var userId = Guid.NewGuid();
        var notification = Notification.Create(userId, "Follow", "New Follower", "User followed you", "ref-123");

        notification.UserId.Should().Be(userId);
        notification.Type.Should().Be("Follow");
        notification.ReferenceId.Should().Be("ref-123");
    }
}
