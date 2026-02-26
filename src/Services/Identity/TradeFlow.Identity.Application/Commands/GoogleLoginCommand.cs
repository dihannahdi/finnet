using MediatR;
using TradeFlow.Common.Application;

namespace TradeFlow.Identity.Application.Commands;

public record GoogleLoginCommand : IRequest<Result<AuthResponse>>
{
    public string IdToken { get; init; } = default!;
}
