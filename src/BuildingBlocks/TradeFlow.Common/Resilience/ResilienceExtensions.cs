using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;

namespace TradeFlow.Common.Resilience;

/// <summary>
/// Polly v8 resilience pipelines for service-to-service communication.
/// Implements retry with exponential backoff and circuit breaker patterns.
/// </summary>
public static class ResilienceExtensions
{
    /// <summary>
    /// Creates a standard resilience pipeline for HTTP requests with retry + circuit breaker.
    /// </summary>
    public static ResiliencePipeline<HttpResponseMessage> CreateHttpResiliencePipeline(ILoggerFactory? loggerFactory = null)
    {
        var logger = loggerFactory?.CreateLogger("Polly.Resilience");

        return new ResiliencePipelineBuilder<HttpResponseMessage>()
            // 1. Retry: exponential backoff, 3 attempts
            .AddRetry(new RetryStrategyOptions<HttpResponseMessage>
            {
                MaxRetryAttempts = 3,
                BackoffType = DelayBackoffType.Exponential,
                Delay = TimeSpan.FromMilliseconds(500),
                ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
                    .Handle<HttpRequestException>()
                    .HandleResult(r => (int)r.StatusCode >= 500),
                OnRetry = args =>
                {
                    logger?.LogWarning(
                        "Retry {AttemptNumber} after {Delay}ms. Reason: {Reason}",
                        args.AttemptNumber,
                        args.RetryDelay.TotalMilliseconds,
                        args.Outcome.Exception?.Message ?? args.Outcome.Result?.StatusCode.ToString());
                    return default;
                }
            })
            // 2. Circuit breaker: open after 50% failure rate in 30s window (min 5 requests)
            .AddCircuitBreaker(new CircuitBreakerStrategyOptions<HttpResponseMessage>
            {
                FailureRatio = 0.5,
                SamplingDuration = TimeSpan.FromSeconds(30),
                MinimumThroughput = 5,
                BreakDuration = TimeSpan.FromSeconds(30),
                ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
                    .Handle<HttpRequestException>()
                    .HandleResult(r => (int)r.StatusCode >= 500),
                OnOpened = args =>
                {
                    logger?.LogError("Circuit breaker OPENED for {Duration}s", args.BreakDuration.TotalSeconds);
                    return default;
                },
                OnClosed = _ =>
                {
                    logger?.LogInformation("Circuit breaker CLOSED. Service recovered.");
                    return default;
                },
                OnHalfOpened = _ =>
                {
                    logger?.LogInformation("Circuit breaker HALF-OPEN. Testing...");
                    return default;
                }
            })
            .Build();
    }

    /// <summary>
    /// Creates a generic resilience pipeline (non-HTTP) for any operation.
    /// Useful for database calls, cache operations, etc.
    /// </summary>
    public static ResiliencePipeline CreateGenericResiliencePipeline(ILoggerFactory? loggerFactory = null)
    {
        var logger = loggerFactory?.CreateLogger("Polly.Resilience");

        return new ResiliencePipelineBuilder()
            .AddRetry(new RetryStrategyOptions
            {
                MaxRetryAttempts = 3,
                BackoffType = DelayBackoffType.Exponential,
                Delay = TimeSpan.FromMilliseconds(200),
                OnRetry = args =>
                {
                    logger?.LogWarning(
                        "Generic retry {AttemptNumber} after {Delay}ms",
                        args.AttemptNumber, args.RetryDelay.TotalMilliseconds);
                    return default;
                }
            })
            .Build();
    }
}


