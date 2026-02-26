using FluentValidation;
using MediatR;
using TradeFlow.Common.Application;

namespace TradeFlow.Identity.Application.Commands;

public record LoginCommand : IRequest<Result<AuthResponse>>
{
    public string Email { get; init; } = default!;
    public string Password { get; init; } = default!;
}

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
    }
}
