using MediatR;
using TradeFlow.Common.Application;
using TradeFlow.Social.Application.Commands;
using TradeFlow.Social.Domain.Interfaces;

namespace TradeFlow.Social.Application.Queries;

public record GetFeedQuery : IRequest<Result<PagedResult<TradeIdeaDto>>>
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}

public record GetNotificationsQuery : IRequest<Result<List<NotificationDto>>>
{
    public Guid UserId { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}

public record NotificationDto
{
    public Guid Id { get; init; }
    public string Type { get; init; } = default!;
    public string Title { get; init; } = default!;
    public string Message { get; init; } = default!;
    public bool IsRead { get; init; }
    public DateTime CreatedAt { get; init; }
}

public class GetFeedQueryHandler : IRequestHandler<GetFeedQuery, Result<PagedResult<TradeIdeaDto>>>
{
    private readonly ITradeIdeaRepository _repository;

    public GetFeedQueryHandler(ITradeIdeaRepository repository) => _repository = repository;

    public async Task<Result<PagedResult<TradeIdeaDto>>> Handle(GetFeedQuery request, CancellationToken cancellationToken)
    {
        var ideas = await _repository.GetFeedAsync(request.Page, request.PageSize, cancellationToken);
        var total = await _repository.GetTotalCountAsync(cancellationToken);

        var dtos = ideas.Select(i => new TradeIdeaDto
        {
            Id = i.Id, AuthorId = i.AuthorId, AuthorName = i.AuthorName,
            Symbol = i.Symbol, Direction = i.Direction, Title = i.Title,
            Content = i.Content, TargetPrice = i.TargetPrice, StopLoss = i.StopLoss,
            LikesCount = i.LikesCount, CommentsCount = i.CommentsCount, CreatedAt = i.CreatedAt
        }).ToList();

        return Result<PagedResult<TradeIdeaDto>>.Success(
            new PagedResult<TradeIdeaDto>(dtos, total, request.Page, request.PageSize));
    }
}

public class GetNotificationsQueryHandler : IRequestHandler<GetNotificationsQuery, Result<List<NotificationDto>>>
{
    private readonly INotificationRepository _repository;

    public GetNotificationsQueryHandler(INotificationRepository repository) => _repository = repository;

    public async Task<Result<List<NotificationDto>>> Handle(GetNotificationsQuery request, CancellationToken cancellationToken)
    {
        var notifications = await _repository.GetByUserIdAsync(request.UserId, request.Page, request.PageSize, cancellationToken);
        var dtos = notifications.Select(n => new NotificationDto
        {
            Id = n.Id, Type = n.Type, Title = n.Title, Message = n.Message,
            IsRead = n.IsRead, CreatedAt = n.CreatedAt
        }).ToList();

        return Result<List<NotificationDto>>.Success(dtos);
    }
}
