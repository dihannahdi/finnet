# ADR-001: RabbitMQ over Kafka for Event Messaging

## Status
**Accepted** — January 2025

## Context
TradeFlow requires asynchronous inter-service communication for trade execution events, social notifications, and market price updates. The two leading contenders are **RabbitMQ** and **Apache Kafka**.

Key requirements:
- Low-latency event delivery for trade confirmations
- Fan-out pattern for notifications (one trade → multiple consumers)
- Simple operational setup for a demo/portfolio project
- Strong .NET ecosystem support via MassTransit

## Decision
We chose **RabbitMQ** with **MassTransit** as the transport abstraction.

## Rationale

| Criteria | RabbitMQ | Kafka |
|----------|----------|-------|
| Latency | Sub-millisecond message delivery | Higher (batching optimized for throughput) |
| .NET Support | MassTransit first-class transport | MassTransit supports Kafka but RabbitMQ is the primary transport |
| Operational Complexity | Single Docker container | Requires ZooKeeper + brokers |
| Routing | Flexible exchange/queue bindings | Topic-partition model |
| Message Patterns | Publish/Subscribe, Request/Reply, Saga | Primarily stream processing |
| Throughput | Sufficient for < 100k msg/sec | Optimized for millions msg/sec |

**RabbitMQ wins** because:
1. **MassTransit integration** — RabbitMQ is MassTransit's primary and most mature transport
2. **Operational simplicity** — single container, no ZooKeeper dependency
3. **Routing flexibility** — exchange bindings support our fan-out notification pattern
4. **Appropriate scale** — our trade volumes don't require Kafka's throughput capabilities
5. **Transactional outbox** — MassTransit's EF Core outbox works seamlessly with RabbitMQ

## Consequences
- **Positive**: Simpler infrastructure, faster development, excellent .NET tooling
- **Positive**: MassTransit outbox pattern ensures exactly-once delivery semantics
- **Negative**: If we needed to replay events (event sourcing), Kafka's log-based storage would be superior
- **Negative**: Not suitable if the system scales beyond 100k messages/second
- **Mitigation**: MassTransit's transport abstraction allows swapping to Kafka with minimal code changes

## References
- [MassTransit Documentation](https://masstransit.io/)
- [RabbitMQ vs Kafka](https://www.confluent.io/blog/kafka-vs-rabbitmq/)
