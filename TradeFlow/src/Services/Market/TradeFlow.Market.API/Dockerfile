FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/BuildingBlocks/TradeFlow.Common/TradeFlow.Common.csproj", "src/BuildingBlocks/TradeFlow.Common/"]
COPY ["src/BuildingBlocks/TradeFlow.MessageContracts/TradeFlow.MessageContracts.csproj", "src/BuildingBlocks/TradeFlow.MessageContracts/"]
COPY ["src/Services/Market/TradeFlow.Market.Domain/TradeFlow.Market.Domain.csproj", "src/Services/Market/TradeFlow.Market.Domain/"]
COPY ["src/Services/Market/TradeFlow.Market.Application/TradeFlow.Market.Application.csproj", "src/Services/Market/TradeFlow.Market.Application/"]
COPY ["src/Services/Market/TradeFlow.Market.Infrastructure/TradeFlow.Market.Infrastructure.csproj", "src/Services/Market/TradeFlow.Market.Infrastructure/"]
COPY ["src/Services/Market/TradeFlow.Market.API/TradeFlow.Market.API.csproj", "src/Services/Market/TradeFlow.Market.API/"]
COPY ["Directory.Build.props", "./"]
RUN dotnet restore "src/Services/Market/TradeFlow.Market.API/TradeFlow.Market.API.csproj"
COPY . .
WORKDIR "/src/src/Services/Market/TradeFlow.Market.API"
RUN dotnet build "TradeFlow.Market.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TradeFlow.Market.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:80
ENTRYPOINT ["dotnet", "TradeFlow.Market.API.dll"]
