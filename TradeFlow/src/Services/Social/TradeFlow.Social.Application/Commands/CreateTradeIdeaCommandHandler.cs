using MassTransit;
using MediatR;
using TradeFlow.Common.Application;
using TradeFlow.MessageContracts.Events;
using TradeFlow.Social.Domain.Entities;
using TradeFlow.Social.Domain.Interfaces;

namespace TradeFlow.Social.Application.Commands;

public class CreateTradeIdeaCommandHandler : IRequestHandler<CreateTradeIdeaCommand, Result<TradeIdeaDto>>
{
    private readonly ITradeIdeaRepository _repository;
    private readonly IPublishEndpoint _publishEndpoint;

    public CreateTradeIdeaCommandHandler(ITradeIdeaRepository repository, IPublishEndpoint publishEndpoint)
    {
        _repository = repository;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<Result<TradeIdeaDto>> Handle(CreateTradeIdeaCommand request, CancellationToken cancellationToken)
    {
        var idea = TradeIdea.Create(
            request.AuthorId, request.AuthorName, request.Symbol,
            request.Direction, request.Title, request.Content,
            request.TargetPrice, request.StopLoss);

        await _repository.AddAsync(idea, cancellationToken);

        await _publishEndpoint.Publish(new TradeIdeaPublished
        {
            IdeaId = idea.Id,
            AuthorId = idea.AuthorId,
            Symbol = idea.Symbol,
            Direction = idea.Direction,
            Content = idea.Content,
            PublishedAt = idea.CreatedAt
        }, cancellationToken);

        return Result<TradeIdeaDto>.Success(MapToDto(idea));
    }

    private static TradeIdeaDto MapToDto(TradeIdea idea) => new()
    {
        Id = idea.Id, AuthorId = idea.AuthorId, AuthorName = idea.AuthorName,
        Symbol = idea.Symbol, Direction = idea.Direction, Title = idea.Title,
        Content = idea.Content, TargetPrice = idea.TargetPrice, StopLoss = idea.StopLoss,
        LikesCount = idea.LikesCount, CommentsCount = idea.CommentsCount, CreatedAt = idea.CreatedAt
    };
}
