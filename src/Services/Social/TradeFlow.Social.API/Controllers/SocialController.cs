using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TradeFlow.Social.Application.Commands;
using TradeFlow.Social.Application.Queries;

namespace TradeFlow.Social.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SocialController : ControllerBase
{
    private readonly IMediator _mediator;
    public SocialController(IMediator mediator) => _mediator = mediator;

    [HttpGet("feed")]
    public async Task<IActionResult> GetFeed([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetFeedQuery { Page = page, PageSize = pageSize }, ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    [Authorize]
    [HttpPost("ideas")]
    public async Task<IActionResult> CreateIdea([FromBody] CreateIdeaRequest request, CancellationToken ct)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();
        var userName = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value ?? "Unknown";

        var command = new CreateTradeIdeaCommand
        {
            AuthorId = userId.Value, AuthorName = userName, Symbol = request.Symbol,
            Direction = request.Direction, Title = request.Title, Content = request.Content,
            TargetPrice = request.TargetPrice, StopLoss = request.StopLoss
        };
        var result = await _mediator.Send(command, ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    [Authorize]
    [HttpPost("follow/{followingId:guid}")]
    public async Task<IActionResult> Follow(Guid followingId, CancellationToken ct)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        var result = await _mediator.Send(new FollowUserCommand { FollowerId = userId.Value, FollowingId = followingId }, ct);
        return result.IsSuccess ? Ok(new { message = result.Value }) : BadRequest(new { error = result.Error });
    }

    [Authorize]
    [HttpDelete("follow/{followingId:guid}")]
    public async Task<IActionResult> Unfollow(Guid followingId, CancellationToken ct)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        var result = await _mediator.Send(new UnfollowUserCommand { FollowerId = userId.Value, FollowingId = followingId }, ct);
        return result.IsSuccess ? Ok(new { message = result.Value }) : BadRequest(new { error = result.Error });
    }

    [Authorize]
    [HttpGet("notifications")]
    public async Task<IActionResult> GetNotifications([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        var result = await _mediator.Send(new GetNotificationsQuery { UserId = userId.Value, Page = page, PageSize = pageSize }, ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    private Guid? GetUserId()
    {
        var claim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst("sub")?.Value;
        return Guid.TryParse(claim, out var id) ? id : null;
    }
}

public record CreateIdeaRequest
{
    public string Symbol { get; init; } = default!;
    public string Direction { get; init; } = default!;
    public string Title { get; init; } = default!;
    public string Content { get; init; } = default!;
    public decimal? TargetPrice { get; init; }
    public decimal? StopLoss { get; init; }
}
