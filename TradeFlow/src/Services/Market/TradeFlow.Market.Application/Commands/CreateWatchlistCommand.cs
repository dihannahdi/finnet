using FluentValidation;
using MediatR;
using TradeFlow.Common.Application;
using TradeFlow.Market.Domain.Entities;
using TradeFlow.Market.Domain.Interfaces;

namespace TradeFlow.Market.Application.Commands;

public record CreateWatchlistCommand : IRequest<Result<WatchlistDto>>
{
    public Guid UserId { get; init; }
    public string Name { get; init; } = default!;
    public List<string> Symbols { get; init; } = new();
}

public record WatchlistDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = default!;
    public List<string> Symbols { get; init; } = new();
    public DateTime CreatedAt { get; init; }
}

public class CreateWatchlistCommandValidator : AbstractValidator<CreateWatchlistCommand>
{
    public CreateWatchlistCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
    }
}

public class CreateWatchlistCommandHandler : IRequestHandler<CreateWatchlistCommand, Result<WatchlistDto>>
{
    private readonly IWatchlistRepository _repository;

    public CreateWatchlistCommandHandler(IWatchlistRepository repository) => _repository = repository;

    public async Task<Result<WatchlistDto>> Handle(CreateWatchlistCommand request, CancellationToken cancellationToken)
    {
        var watchlist = Watchlist.Create(request.UserId, request.Name);
        foreach (var symbol in request.Symbols)
            watchlist.AddSymbol(symbol);

        await _repository.AddAsync(watchlist, cancellationToken);

        return Result<WatchlistDto>.Success(new WatchlistDto
        {
            Id = watchlist.Id,
            Name = watchlist.Name,
            Symbols = watchlist.Items.Select(i => i.Symbol).ToList(),
            CreatedAt = watchlist.CreatedAt
        });
    }
}
