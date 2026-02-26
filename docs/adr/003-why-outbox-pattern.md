# ADR-003: Transactional Outbox Pattern for Reliable Messaging

## Status
**Accepted** — January 2025

## Context
In a microservices architecture, a common challenge is ensuring **atomicity between database writes and message publishing**. When a service updates its database and publishes an event, two failure modes exist:

1. **Database commits but message publish fails** → downstream services never receive the event
2. **Message publishes but database transaction rolls back** → downstream services process a phantom event

This is the classic "dual write" problem.

## Decision
We implement the **Transactional Outbox Pattern** using **MassTransit's Entity Framework Core Outbox**.

## How It Works

```
┌─────────────────────────────────────────────┐
│           Service Database                   │
│                                              │
│  ┌──────────┐  ┌──────────────────────────┐ │
│  │ Business  │  │   OutboxMessage Table    │ │
│  │  Tables   │  │   (same transaction)     │ │
│  └──────────┘  └──────────────────────────┘ │
│                         │                    │
└─────────────────────────│────────────────────┘
                          │ Background delivery
                          ▼
                   ┌──────────────┐
                   │  RabbitMQ    │
                   └──────────────┘
```

1. Business logic writes to domain tables **and** outbox table in a **single database transaction**
2. A background process (`UseBusOutbox()`) polls the outbox table and delivers messages to RabbitMQ
3. Successfully delivered messages are marked as processed

## Implementation

```csharp
// DbContext — register outbox entities
modelBuilder.AddInboxStateEntity();
modelBuilder.AddOutboxMessageEntity();
modelBuilder.AddOutboxStateEntity();

// MassTransit configuration
cfg.AddEntityFrameworkOutbox<PortfolioDbContext>(o =>
{
    o.UsePostgres();
    o.UseBusOutbox();
});
```

Applied to: **Portfolio Service** and **Social Service** (the two services that publish integration events).

## Rationale
1. **Guaranteed delivery** — messages survive service crashes because they're persisted in the database
2. **Atomic consistency** — business data and outbox messages commit in the same transaction
3. **Idempotent consumers** — MassTransit's inbox pattern deduplicates already-processed messages
4. **No distributed transactions** — avoids 2PC (two-phase commit) complexity
5. **Framework support** — MassTransit's built-in implementation is battle-tested

## Alternatives Considered

| Alternative | Why Rejected |
|-------------|-------------|
| CDC (Change Data Capture) | Requires Debezium + Kafka Connect — excessive infrastructure |
| Polling Publisher (custom) | Reinventing the wheel; MassTransit provides this out of the box |
| Saga/Compensation | Appropriate for multi-step workflows, overkill for single event publishing |
| No outbox (fire-and-forget) | Unacceptable message loss risk for trade and notification events |

## Consequences
- **Positive**: Zero message loss for trade execution and social notification events
- **Positive**: No additional infrastructure beyond existing PostgreSQL + RabbitMQ
- **Positive**: Automatic retry and deduplication via MassTransit inbox/outbox
- **Negative**: Slight increase in database storage (outbox tables)
- **Negative**: Small latency increase (outbox polling interval, default ~1 second)
- **Negative**: Requires EF Core (already our ORM choice)

## References
- [MassTransit Transactional Outbox](https://masstransit.io/documentation/patterns/transactional-outbox)
- [Microservices Patterns — Chris Richardson](https://microservices.io/patterns/data/transactional-outbox.html)
