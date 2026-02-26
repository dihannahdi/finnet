using MediatR;
using TradeFlow.Common.Application;

namespace TradeFlow.Identity.Application.Queries;

public record GetUserQuery : IRequest<Result<UserDto>>
{
    public Guid UserId { get; init; }
}

public record UserDto
{
    public Guid Id { get; init; }
    public string Email { get; init; } = default!;
    public string DisplayName { get; init; } = default!;
    public string? AvatarUrl { get; init; }
    public string Role { get; init; } = default!;
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? LastLoginAt { get; init; }
}
