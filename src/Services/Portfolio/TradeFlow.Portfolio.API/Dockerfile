FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/BuildingBlocks/TradeFlow.Common/TradeFlow.Common.csproj", "src/BuildingBlocks/TradeFlow.Common/"]
COPY ["src/BuildingBlocks/TradeFlow.MessageContracts/TradeFlow.MessageContracts.csproj", "src/BuildingBlocks/TradeFlow.MessageContracts/"]
COPY ["src/Services/Portfolio/TradeFlow.Portfolio.Domain/TradeFlow.Portfolio.Domain.csproj", "src/Services/Portfolio/TradeFlow.Portfolio.Domain/"]
COPY ["src/Services/Portfolio/TradeFlow.Portfolio.Application/TradeFlow.Portfolio.Application.csproj", "src/Services/Portfolio/TradeFlow.Portfolio.Application/"]
COPY ["src/Services/Portfolio/TradeFlow.Portfolio.Infrastructure/TradeFlow.Portfolio.Infrastructure.csproj", "src/Services/Portfolio/TradeFlow.Portfolio.Infrastructure/"]
COPY ["src/Services/Portfolio/TradeFlow.Portfolio.API/TradeFlow.Portfolio.API.csproj", "src/Services/Portfolio/TradeFlow.Portfolio.API/"]
COPY ["Directory.Build.props", "./"]
RUN dotnet restore "src/Services/Portfolio/TradeFlow.Portfolio.API/TradeFlow.Portfolio.API.csproj"
COPY . .
WORKDIR "/src/src/Services/Portfolio/TradeFlow.Portfolio.API"
RUN dotnet build "TradeFlow.Portfolio.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TradeFlow.Portfolio.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:80
ENTRYPOINT ["dotnet", "TradeFlow.Portfolio.API.dll"]
