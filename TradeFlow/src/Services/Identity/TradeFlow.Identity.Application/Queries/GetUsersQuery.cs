using MediatR;
using TradeFlow.Common.Application;

namespace TradeFlow.Identity.Application.Queries;

public record GetUsersQuery : IRequest<Result<PagedResult<UserDto>>>
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}
