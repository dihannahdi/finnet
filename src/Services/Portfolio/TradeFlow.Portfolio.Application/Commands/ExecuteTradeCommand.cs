using FluentValidation;
using MediatR;
using TradeFlow.Common.Application;

namespace TradeFlow.Portfolio.Application.Commands;

public record ExecuteTradeCommand : IRequest<Result<TradeDto>>
{
    public Guid UserId { get; init; }
    public string Symbol { get; init; } = default!;
    public string Side { get; init; } = default!; // Buy or Sell
    public decimal Quantity { get; init; }
    public decimal Price { get; init; }
}

public record TradeDto
{
    public Guid TradeId { get; init; }
    public string Symbol { get; init; } = default!;
    public string Side { get; init; } = default!;
    public decimal Quantity { get; init; }
    public decimal Price { get; init; }
    public decimal TotalValue { get; init; }
    public DateTime ExecutedAt { get; init; }
}

public class ExecuteTradeCommandValidator : AbstractValidator<ExecuteTradeCommand>
{
    public ExecuteTradeCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Symbol).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Side).NotEmpty().Must(s => s == "Buy" || s == "Sell").WithMessage("Side must be 'Buy' or 'Sell'");
        RuleFor(x => x.Quantity).GreaterThan(0);
        RuleFor(x => x.Price).GreaterThan(0);
    }
}
