using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TradeFlow.Market.Domain.Entities;
using TradeFlow.Market.Domain.Interfaces;
using TradeFlow.MessageContracts.Events;

namespace TradeFlow.Market.Infrastructure.Services;

public class FinnhubWebSocketService : BackgroundService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<FinnhubWebSocketService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private ClientWebSocket? _webSocket;

    private static readonly string[] DefaultSymbols = { "AAPL", "GOOGL", "MSFT", "AMZN", "TSLA", "NVDA", "META", "BINANCE:BTCUSDT", "BINANCE:ETHUSDT" };

    public FinnhubWebSocketService(
        IConfiguration configuration,
        ILogger<FinnhubWebSocketService> logger,
        IServiceProvider serviceProvider)
    {
        _configuration = configuration;
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var apiKey = _configuration["Finnhub:ApiKey"];
        if (string.IsNullOrEmpty(apiKey))
        {
            _logger.LogWarning("Finnhub API key not configured. Using simulated market data.");
            await SimulateMarketData(stoppingToken);
            return;
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _webSocket = new ClientWebSocket();
                await _webSocket.ConnectAsync(new Uri($"wss://ws.finnhub.io?token={apiKey}"), stoppingToken);
                _logger.LogInformation("Connected to Finnhub WebSocket");

                foreach (var symbol in DefaultSymbols)
                {
                    var subscribeMsg = JsonSerializer.Serialize(new { type = "subscribe", symbol });
                    await _webSocket.SendAsync(Encoding.UTF8.GetBytes(subscribeMsg), WebSocketMessageType.Text, true, stoppingToken);
                }

                await ReceiveMessages(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Finnhub WebSocket error. Reconnecting in 5 seconds...");
                await Task.Delay(5000, stoppingToken);
            }
        }
    }

    private async Task ReceiveMessages(CancellationToken stoppingToken)
    {
        var buffer = new byte[4096];
        while (_webSocket?.State == WebSocketState.Open && !stoppingToken.IsCancellationRequested)
        {
            var result = await _webSocket.ReceiveAsync(buffer, stoppingToken);
            if (result.MessageType == WebSocketMessageType.Close) break;

            var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
            await ProcessFinnhubMessage(message);
        }
    }

    private async Task ProcessFinnhubMessage(string message)
    {
        try
        {
            using var doc = JsonDocument.Parse(message);
            if (doc.RootElement.GetProperty("type").GetString() != "trade") return;

            var data = doc.RootElement.GetProperty("data");
            foreach (var trade in data.EnumerateArray())
            {
                var symbol = trade.GetProperty("s").GetString()!;
                var price = trade.GetProperty("p").GetDecimal();
                var volume = trade.GetProperty("v").GetDecimal();

                await PublishPriceUpdate(symbol, price, volume);
            }
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Failed to parse Finnhub message");
        }
    }

    private async Task SimulateMarketData(CancellationToken stoppingToken)
    {
        var random = new Random();
        var basePrices = new Dictionary<string, decimal>
        {
            ["AAPL"] = 178.50m, ["GOOGL"] = 141.80m, ["MSFT"] = 378.90m,
            ["AMZN"] = 178.25m, ["TSLA"] = 248.50m, ["NVDA"] = 875.30m,
            ["META"] = 505.75m, ["BINANCE:BTCUSDT"] = 67500m, ["BINANCE:ETHUSDT"] = 3450m
        };

        // Seed initial data
        using (var scope = _serviceProvider.CreateScope())
        {
            var repo = scope.ServiceProvider.GetRequiredService<IMarketPriceRepository>();
            foreach (var (symbol, price) in basePrices)
            {
                var existing = await repo.GetBySymbolAsync(symbol);
                if (existing is null)
                {
                    var assetType = symbol.StartsWith("BINANCE:") ? "Crypto" : "Stock";
                    var name = symbol.Replace("BINANCE:", "");
                    var mp = MarketPrice.Create(symbol, name, assetType, price);
                    await repo.AddAsync(mp);
                }
            }
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            foreach (var (symbol, basePrice) in basePrices)
            {
                var change = basePrice * (decimal)(random.NextDouble() * 0.02 - 0.01);
                var newPrice = Math.Round(basePrice + change, 2);
                var volume = random.Next(100, 10000);

                await PublishPriceUpdate(symbol, newPrice, volume);
            }

            await Task.Delay(3000, stoppingToken);
        }
    }

    private async Task PublishPriceUpdate(string symbol, decimal price, decimal volume)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<IMarketPriceRepository>();
            var cache = scope.ServiceProvider.GetRequiredService<IMarketDataCache>();
            var bus = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

            var existing = await repo.GetBySymbolAsync(symbol);
            if (existing != null)
            {
                existing.UpdatePrice(price, volume);
                await repo.UpdateAsync(existing);
                await cache.SetPriceAsync(existing);

                await bus.Publish(new MarketPriceUpdated
                {
                    Symbol = symbol,
                    Price = price,
                    Change = existing.Change,
                    ChangePercent = existing.ChangePercent,
                    Volume = volume,
                    Timestamp = DateTime.UtcNow
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to publish price update for {Symbol}", symbol);
        }
    }
}
