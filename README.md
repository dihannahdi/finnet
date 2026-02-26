# TradeFlow — Production-Grade Trading Portfolio Platform

[![Build & Test](https://github.com/dihannahdi/finnet/actions/workflows/ci-cd.yml/badge.svg)](https://github.com/dihannahdi/finnet/actions/workflows/ci-cd.yml)

A **production-grade trading portfolio platform** built with **.NET 8 microservices**, designed for demo and showcase purposes. Features real-time market data, portfolio management, social trading, and a unified API gateway.

## 🏗️ Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                    API Gateway (YARP)                       │
│                    Port: 5000                               │
├────────────┬────────────┬──────────────┬───────────────────┤
│  Identity  │   Market   │  Portfolio   │      Social       │
│  Service   │   Service  │   Service    │     Service       │
│  :5001     │   :5002    │   :5003      │     :5004         │
├────────────┴────────────┴──────────────┴───────────────────┤
│           Message Bus (RabbitMQ :5672/:15672)               │
├─────────────────┬──────────────────┬───────────────────────┤
│   PostgreSQL    │      Redis       │      SignalR          │
│   :5432         │      :6379       │   (WebSocket Hubs)    │
└─────────────────┴──────────────────┴───────────────────────┘
```

## 🛠️ Tech Stack

| Layer | Technology |
|---|---|
| Framework | .NET 8 / ASP.NET Core |
| Architecture | Clean Architecture, CQRS |
| Messaging | MassTransit + RabbitMQ |
| Mediator | MediatR 12.x |
| Database | PostgreSQL + EF Core 8 |
| Caching | Redis (IDistributedCache) |
| Real-time | SignalR WebSocket Hubs |
| API Gateway | YARP Reverse Proxy |
| Auth | JWT + Google OAuth2 |
| Validation | FluentValidation |
| Logging | Serilog (structured) |
| Resilience | Polly (retry, circuit breaker) |
| Containers | Docker + Docker Compose |
| CI/CD | GitHub Actions |
| Testing | xUnit, Moq, FluentAssertions, Testcontainers |

## 📦 Services

### Identity Service (`:5001`)
- User registration & login with JWT tokens
- Google OAuth2 integration
- Refresh token rotation
- Role-based access control (User, Admin)

### Market Service (`:5002`)
- Real-time market prices (Finnhub WebSocket / simulated)
- SignalR hub for live price streaming
- Watchlist management
- Redis caching layer

### Portfolio Service (`:5003`)
- Paper trading with $100K demo cash
- Buy/Sell execution with average price tracking
- Real-time P&L calculation
- Trade history with pagination

### Social Service (`:5004`)
- Trade ideas feed (create, like, comment)
- Follow/unfollow traders
- Real-time notifications via SignalR
- Activity feed from followed traders

### API Gateway (`:5000`)
- YARP reverse proxy routing
- Centralized JWT validation
- Rate limiting (100 req/min per IP)
- Swagger UI aggregation

## 🚀 Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)

### Run with Docker Compose
```bash
git clone https://github.com/dihannahdi/finnet.git
cd finnet/TradeFlow
docker-compose up --build
```

### Run Locally
```bash
# Start infrastructure
docker-compose up -d postgres redis rabbitmq

# Run each service
dotnet run --project src/Services/Identity/TradeFlow.Identity.API
dotnet run --project src/Services/Market/TradeFlow.Market.API
dotnet run --project src/Services/Portfolio/TradeFlow.Portfolio.API
dotnet run --project src/Services/Social/TradeFlow.Social.API
dotnet run --project src/Gateway/TradeFlow.Gateway
```

### API Endpoints

| Endpoint | Description |
|---|---|
| `POST /api/auth/register` | Register new user |
| `POST /api/auth/login` | Login & get JWT |
| `POST /api/auth/refresh` | Refresh access token |
| `GET /api/market/prices` | Get all market prices |
| `GET /api/market/prices/{symbol}` | Get price by symbol |
| `POST /api/portfolio/trade` | Execute a trade |
| `GET /api/portfolio` | Get portfolio |
| `GET /api/social/feed` | Get trade ideas feed |
| `POST /api/social/ideas` | Post trade idea |
| `POST /api/social/follow/{id}` | Follow a trader |
| `GET /health` | Health check |

### Swagger UI
- Gateway: http://localhost:5000/swagger
- Identity: http://localhost:5001/swagger
- Market: http://localhost:5002/swagger
- Portfolio: http://localhost:5003/swagger
- Social: http://localhost:5004/swagger

## 🧪 Testing
```bash
dotnet test --verbosity normal
```

## 📁 Project Structure
```
TradeFlow/
├── src/
│   ├── BuildingBlocks/
│   │   ├── TradeFlow.Common/          # Shared domain primitives & behaviors
│   │   └── TradeFlow.MessageContracts/ # Integration events
│   ├── Gateway/
│   │   └── TradeFlow.Gateway/         # YARP API Gateway
│   └── Services/
│       ├── Identity/                   # Auth microservice
│       │   ├── TradeFlow.Identity.Domain/
│       │   ├── TradeFlow.Identity.Application/
│       │   ├── TradeFlow.Identity.Infrastructure/
│       │   └── TradeFlow.Identity.API/
│       ├── Market/                     # Market data microservice
│       ├── Portfolio/                  # Trading microservice
│       └── Social/                     # Social feed microservice
├── tests/
│   ├── TradeFlow.Identity.Tests/
│   ├── TradeFlow.Market.Tests/
│   ├── TradeFlow.Portfolio.Tests/
│   └── TradeFlow.Social.Tests/
├── docker-compose.yml
├── Directory.Build.props
└── TradeFlow.sln
```

## 📄 License
MIT License — For educational and demo purposes.
