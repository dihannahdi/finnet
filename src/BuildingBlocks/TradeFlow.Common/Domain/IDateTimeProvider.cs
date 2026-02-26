namespace TradeFlow.Common.Domain;

/// <summary>
/// Abstraction over DateTime.UtcNow to enable deterministic testing.
/// Inject IDateTimeProvider instead of calling DateTime.UtcNow directly.
/// </summary>
public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}

/// <summary>
/// Production implementation that delegates to DateTime.UtcNow.
/// </summary>
public sealed class SystemDateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
