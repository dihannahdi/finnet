using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TradeFlow.Portfolio.Application.Commands;
using TradeFlow.Portfolio.Application.Queries;

namespace TradeFlow.Portfolio.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PortfolioController : ControllerBase
{
    private readonly IMediator _mediator;
    public PortfolioController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetPortfolio(CancellationToken ct)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        var result = await _mediator.Send(new GetPortfolioQuery { UserId = userId.Value }, ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { error = result.Error });
    }

    [HttpPost("trade")]
    public async Task<IActionResult> ExecuteTrade([FromBody] ExecuteTradeRequest request, CancellationToken ct)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        var command = new ExecuteTradeCommand
        {
            UserId = userId.Value,
            Symbol = request.Symbol,
            Side = request.Side,
            Quantity = request.Quantity,
            Price = request.Price
        };

        var result = await _mediator.Send(command, ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    [HttpGet("trades")]
    public async Task<IActionResult> GetTradeHistory([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
    {
        var userId = GetUserId();
        if (userId is null) return Unauthorized();

        var result = await _mediator.Send(new GetTradeHistoryQuery { UserId = userId.Value, Page = page, PageSize = pageSize }, ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { error = result.Error });
    }

    private Guid? GetUserId()
    {
        var claim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst("sub")?.Value;
        return Guid.TryParse(claim, out var id) ? id : null;
    }
}

public record ExecuteTradeRequest
{
    public string Symbol { get; init; } = default!;
    public string Side { get; init; } = default!;
    public decimal Quantity { get; init; }
    public decimal Price { get; init; }
}
