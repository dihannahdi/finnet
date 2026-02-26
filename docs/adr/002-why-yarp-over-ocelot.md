# ADR-002: YARP over Ocelot for API Gateway

## Status
**Accepted** — January 2025

## Context
TradeFlow's microservices architecture requires an API Gateway to:
- Route external requests to internal services
- Handle cross-cutting concerns (rate limiting, authentication forwarding)
- Provide a single entry point for the frontend

The main .NET API Gateway options are **YARP (Yet Another Reverse Proxy)** and **Ocelot**.

## Decision
We chose **YARP** as the reverse proxy / API Gateway.

## Rationale

| Criteria | YARP | Ocelot |
|----------|------|--------|
| Maintainer | Microsoft (ASP.NET team) | Community-maintained |
| Performance | Built on Kestrel, highly optimized | Good, but not as optimized |
| Configuration | Flexible, supports hot reload | JSON-based, less flexible |
| .NET Version Support | First-class .NET 8 support | Lagging behind on latest .NET |
| Middleware Integration | Native ASP.NET middleware pipeline | Custom middleware system |
| Production Usage | Used by Azure, Dynamics, Bing | Widely used in community projects |

**YARP wins** because:
1. **Microsoft-backed** — developed and maintained by the ASP.NET team, ensuring long-term support
2. **Performance** — built directly on top of Kestrel, minimizing proxy overhead
3. **Native ASP.NET integration** — rate limiting, authentication, and other middleware work natively
4. **Configuration model** — supports `IConfiguration` hot reload, making dynamic routing trivial
5. **Active development** — regular releases aligned with .NET release cadence

## Configuration Example
```csharp
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
```

Routes are defined declaratively in `appsettings.json`, mapping `/api/identity/**` → Identity Service, `/api/portfolio/**` → Portfolio Service, etc.

## Consequences
- **Positive**: High performance, Microsoft support, native middleware pipeline
- **Positive**: Rate limiting via `Microsoft.AspNetCore.RateLimiting` integrates seamlessly
- **Negative**: Less "batteries included" than Ocelot (e.g., no built-in service discovery)
- **Negative**: Requires more manual configuration for advanced patterns like circuit breaking
- **Mitigation**: Polly resilience pipelines added for retry and circuit breaker patterns

## References
- [YARP Documentation](https://microsoft.github.io/reverse-proxy/)
- [YARP vs Ocelot Comparison](https://devblogs.microsoft.com/dotnet/announcing-yarp-1-0-release/)
