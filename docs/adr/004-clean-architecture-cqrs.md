# ADR-004: Clean Architecture with CQRS per Service

## Status
**Accepted** — January 2025

## Context
Each microservice in TradeFlow needs a consistent internal architecture that:
- Separates business logic from infrastructure concerns
- Supports testability (domain logic testable without databases/message brokers)
- Follows DDD (Domain-Driven Design) tactical patterns
- Scales in complexity as services grow

## Decision
Each service follows **Clean Architecture** with **CQRS (Command Query Responsibility Segregation)** using MediatR.

## Structure
```
Service/
├── Domain/          # Entities, Value Objects, Domain Events, Interfaces
├── Application/     # Commands, Queries, Handlers, DTOs, Validators
├── Infrastructure/  # EF Core, Repository implementations, External services
└── API/             # Controllers, Middleware, Program.cs
```

### Layer Rules
| Layer | Can Reference | Cannot Reference |
|-------|--------------|-----------------|
| Domain | Nothing (except BuildingBlocks) | Application, Infrastructure, API |
| Application | Domain | Infrastructure, API |
| Infrastructure | Domain, Application | API |
| API | All layers | — |

### CQRS via MediatR
- **Commands** modify state: `ExecuteTradeCommand` → `ExecuteTradeCommandHandler`
- **Queries** read state: `GetMarketPriceQuery` → `GetMarketPriceQueryHandler`
- **Pipeline Behaviors** for cross-cutting: validation, logging

## Rationale
1. **Testability** — domain logic is pure C# with no framework dependencies
2. **Dependency inversion** — domain defines interfaces, infrastructure implements them
3. **CQRS clarity** — separate read/write paths make intent explicit
4. **Consistent patterns** — all 4 services follow the same structure
5. **Team scalability** — new developers can navigate any service using the same mental model

## Consequences
- **Positive**: High testability (93 unit tests with no database/broker dependencies)
- **Positive**: Domain events enable loose coupling between aggregates
- **Positive**: MediatR pipeline behaviors provide validation without controller pollution
- **Negative**: More boilerplate (handler classes, DTOs) vs. minimal APIs
- **Negative**: Indirection cost — request flows through multiple layers
- **Mitigation**: Consistent naming conventions and folder structure reduce cognitive load

## References
- [Clean Architecture — Robert C. Martin](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [MediatR](https://github.com/jbogard/MediatR)
