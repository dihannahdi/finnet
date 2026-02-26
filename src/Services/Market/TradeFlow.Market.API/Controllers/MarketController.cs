using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TradeFlow.Market.Application.Commands;
using TradeFlow.Market.Application.Queries;

namespace TradeFlow.Market.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MarketController : ControllerBase
{
    private readonly IMediator _mediator;
    public MarketController(IMediator mediator) => _mediator = mediator;

    [HttpGet("prices")]
    public async Task<IActionResult> GetAllPrices(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetAllPricesQuery(), ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    [HttpGet("prices/{symbol}")]
    public async Task<IActionResult> GetPrice(string symbol, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetMarketPriceQuery { Symbol = symbol }, ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { error = result.Error });
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string q, CancellationToken ct)
    {
        var result = await _mediator.Send(new SearchMarketQuery { Query = q }, ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    [Authorize]
    [HttpPost("watchlists")]
    public async Task<IActionResult> CreateWatchlist([FromBody] CreateWatchlistCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }
}
