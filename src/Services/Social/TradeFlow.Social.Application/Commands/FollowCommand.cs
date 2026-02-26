using MediatR;
using TradeFlow.Common.Application;
using TradeFlow.Social.Domain.Entities;
using TradeFlow.Social.Domain.Interfaces;

namespace TradeFlow.Social.Application.Commands;

public record FollowUserCommand : IRequest<Result<string>>
{
    public Guid FollowerId { get; init; }
    public Guid FollowingId { get; init; }
}

public record UnfollowUserCommand : IRequest<Result<string>>
{
    public Guid FollowerId { get; init; }
    public Guid FollowingId { get; init; }
}

public class FollowUserCommandHandler : IRequestHandler<FollowUserCommand, Result<string>>
{
    private readonly IFollowRepository _followRepo;
    private readonly INotificationRepository _notifRepo;

    public FollowUserCommandHandler(IFollowRepository followRepo, INotificationRepository notifRepo)
    {
        _followRepo = followRepo;
        _notifRepo = notifRepo;
    }

    public async Task<Result<string>> Handle(FollowUserCommand request, CancellationToken cancellationToken)
    {
        var existing = await _followRepo.GetFollowAsync(request.FollowerId, request.FollowingId, cancellationToken);
        if (existing != null)
            return Result<string>.Failure("Already following this user.");

        try
        {
            var follow = Follow.Create(request.FollowerId, request.FollowingId);
            await _followRepo.AddAsync(follow, cancellationToken);

            await _notifRepo.AddAsync(Notification.Create(
                request.FollowingId, "Follow", "New Follower",
                "You have a new follower!", request.FollowerId.ToString()), cancellationToken);

            return Result<string>.Success("Successfully followed user.");
        }
        catch (InvalidOperationException ex)
        {
            return Result<string>.Failure(ex.Message);
        }
    }
}

public class UnfollowUserCommandHandler : IRequestHandler<UnfollowUserCommand, Result<string>>
{
    private readonly IFollowRepository _repository;

    public UnfollowUserCommandHandler(IFollowRepository repository) => _repository = repository;

    public async Task<Result<string>> Handle(UnfollowUserCommand request, CancellationToken cancellationToken)
    {
        var follow = await _repository.GetFollowAsync(request.FollowerId, request.FollowingId, cancellationToken);
        if (follow is null)
            return Result<string>.Failure("Not following this user.");

        await _repository.DeleteAsync(follow, cancellationToken);
        return Result<string>.Success("Successfully unfollowed user.");
    }
}
