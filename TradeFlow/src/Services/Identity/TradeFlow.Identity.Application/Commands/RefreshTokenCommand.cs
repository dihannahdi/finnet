using FluentValidation;
using MediatR;
using TradeFlow.Common.Application;

namespace TradeFlow.Identity.Application.Commands;

public record RefreshTokenCommand : IRequest<Result<AuthResponse>>
{
    public string RefreshToken { get; init; } = default!;
}

public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(x => x.RefreshToken).NotEmpty();
    }
}
