namespace TradeFlow.Social.Domain.Entities;

public class TradeIdea
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid AuthorId { get; private set; }
    public string AuthorName { get; private set; } = default!;
    public string Symbol { get; private set; } = default!;
    public string Direction { get; private set; } = default!; // Bullish, Bearish
    public string Title { get; private set; } = default!;
    public string Content { get; private set; } = default!;
    public decimal? TargetPrice { get; private set; }
    public decimal? StopLoss { get; private set; }
    public int LikesCount { get; private set; }
    public int CommentsCount { get; private set; }
    public bool IsActive { get; private set; } = true;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; private set; }

    private readonly List<Comment> _comments = new();
    public IReadOnlyCollection<Comment> Comments => _comments.AsReadOnly();

    private readonly List<Like> _likes = new();
    public IReadOnlyCollection<Like> Likes => _likes.AsReadOnly();

    private TradeIdea() { }

    public static TradeIdea Create(Guid authorId, string authorName, string symbol, string direction, string title, string content, decimal? targetPrice = null, decimal? stopLoss = null)
    {
        return new TradeIdea
        {
            AuthorId = authorId,
            AuthorName = authorName,
            Symbol = symbol.ToUpperInvariant(),
            Direction = direction,
            Title = title,
            Content = content,
            TargetPrice = targetPrice,
            StopLoss = stopLoss
        };
    }

    public void AddLike(Guid userId)
    {
        if (_likes.Any(l => l.UserId == userId)) return;
        _likes.Add(Like.Create(Id, userId));
        LikesCount++;
    }

    public void RemoveLike(Guid userId)
    {
        var like = _likes.FirstOrDefault(l => l.UserId == userId);
        if (like == null) return;
        _likes.Remove(like);
        LikesCount--;
    }

    public Comment AddComment(Guid userId, string userName, string content)
    {
        var comment = Comment.Create(Id, userId, userName, content);
        _comments.Add(comment);
        CommentsCount++;
        return comment;
    }
}

public class Comment
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid TradeIdeaId { get; private set; }
    public Guid UserId { get; private set; }
    public string UserName { get; private set; } = default!;
    public string Content { get; private set; } = default!;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    private Comment() { }

    public static Comment Create(Guid tradeIdeaId, Guid userId, string userName, string content)
    {
        return new Comment { TradeIdeaId = tradeIdeaId, UserId = userId, UserName = userName, Content = content };
    }
}

public class Like
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid TradeIdeaId { get; private set; }
    public Guid UserId { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    private Like() { }

    public static Like Create(Guid tradeIdeaId, Guid userId)
    {
        return new Like { TradeIdeaId = tradeIdeaId, UserId = userId };
    }
}
