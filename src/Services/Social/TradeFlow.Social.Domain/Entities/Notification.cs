namespace TradeFlow.Social.Domain.Entities;

public class Notification
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid UserId { get; private set; }
    public string Type { get; private set; } = default!; // TradeIdea, Follow, PriceAlert, Trade
    public string Title { get; private set; } = default!;
    public string Message { get; private set; } = default!;
    public string? ReferenceId { get; private set; }
    public bool IsRead { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    private Notification() { }

    public static Notification Create(Guid userId, string type, string title, string message, string? referenceId = null)
    {
        return new Notification { UserId = userId, Type = type, Title = title, Message = message, ReferenceId = referenceId };
    }

    public void MarkAsRead() => IsRead = true;
}
