using FluentValidation;
using MediatR;
using TradeFlow.Common.Application;

namespace TradeFlow.Social.Application.Commands;

public record CreateTradeIdeaCommand : IRequest<Result<TradeIdeaDto>>
{
    public Guid AuthorId { get; init; }
    public string AuthorName { get; init; } = default!;
    public string Symbol { get; init; } = default!;
    public string Direction { get; init; } = default!;
    public string Title { get; init; } = default!;
    public string Content { get; init; } = default!;
    public decimal? TargetPrice { get; init; }
    public decimal? StopLoss { get; init; }
}

public record TradeIdeaDto
{
    public Guid Id { get; init; }
    public Guid AuthorId { get; init; }
    public string AuthorName { get; init; } = default!;
    public string Symbol { get; init; } = default!;
    public string Direction { get; init; } = default!;
    public string Title { get; init; } = default!;
    public string Content { get; init; } = default!;
    public decimal? TargetPrice { get; init; }
    public decimal? StopLoss { get; init; }
    public int LikesCount { get; init; }
    public int CommentsCount { get; init; }
    public DateTime CreatedAt { get; init; }
}

public class CreateTradeIdeaCommandValidator : AbstractValidator<CreateTradeIdeaCommand>
{
    public CreateTradeIdeaCommandValidator()
    {
        RuleFor(x => x.AuthorId).NotEmpty();
        RuleFor(x => x.Symbol).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Direction).NotEmpty().Must(d => d == "Bullish" || d == "Bearish");
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Content).NotEmpty().MaximumLength(5000);
    }
}
