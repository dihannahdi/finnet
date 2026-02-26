using FluentValidation;
using MediatR;
using TradeFlow.Common.Application;

namespace TradeFlow.Identity.Application.Commands;

public record RegisterCommand : IRequest<Result<AuthResponse>>
{
    public string Email { get; init; } = default!;
    public string Password { get; init; } = default!;
    public string DisplayName { get; init; } = default!;
}

public record AuthResponse
{
    public Guid UserId { get; init; }
    public string Email { get; init; } = default!;
    public string DisplayName { get; init; } = default!;
    public string Role { get; init; } = default!;
    public string AccessToken { get; init; } = default!;
    public string RefreshToken { get; init; } = default!;
    public DateTime AccessTokenExpiry { get; init; }
}

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(256);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8).MaximumLength(128);
        RuleFor(x => x.DisplayName).NotEmpty().MinimumLength(2).MaximumLength(100);
    }
}
