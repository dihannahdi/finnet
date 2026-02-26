using MediatR;
using TradeFlow.Common.Application;
using TradeFlow.Identity.Domain.Interfaces;

namespace TradeFlow.Identity.Application.Queries;

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, Result<PagedResult<UserDto>>>
{
    private readonly IUserRepository _userRepository;

    public GetUsersQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<PagedResult<UserDto>>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _userRepository.GetAllAsync(request.Page, request.PageSize, cancellationToken);
        var totalCount = await _userRepository.GetTotalCountAsync(cancellationToken);

        var dtos = users.Select(u => new UserDto
        {
            Id = u.Id,
            Email = u.Email,
            DisplayName = u.DisplayName,
            AvatarUrl = u.AvatarUrl,
            Role = u.Role,
            IsActive = u.IsActive,
            CreatedAt = u.CreatedAt,
            LastLoginAt = u.LastLoginAt
        }).ToList();

        return Result<PagedResult<UserDto>>.Success(
            new PagedResult<UserDto>(dtos, totalCount, request.Page, request.PageSize));
    }
}
