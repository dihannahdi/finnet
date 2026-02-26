namespace TradeFlow.Social.Domain.Entities;

public class Follow
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid FollowerId { get; private set; }
    public Guid FollowingId { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    private Follow() { }

    public static Follow Create(Guid followerId, Guid followingId)
    {
        if (followerId == followingId)
            throw new InvalidOperationException("Cannot follow yourself.");
        return new Follow { FollowerId = followerId, FollowingId = followingId };
    }
}
