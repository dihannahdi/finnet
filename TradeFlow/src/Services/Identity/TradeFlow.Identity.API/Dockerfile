FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/BuildingBlocks/TradeFlow.Common/TradeFlow.Common.csproj", "src/BuildingBlocks/TradeFlow.Common/"]
COPY ["src/BuildingBlocks/TradeFlow.MessageContracts/TradeFlow.MessageContracts.csproj", "src/BuildingBlocks/TradeFlow.MessageContracts/"]
COPY ["src/Services/Identity/TradeFlow.Identity.Domain/TradeFlow.Identity.Domain.csproj", "src/Services/Identity/TradeFlow.Identity.Domain/"]
COPY ["src/Services/Identity/TradeFlow.Identity.Application/TradeFlow.Identity.Application.csproj", "src/Services/Identity/TradeFlow.Identity.Application/"]
COPY ["src/Services/Identity/TradeFlow.Identity.Infrastructure/TradeFlow.Identity.Infrastructure.csproj", "src/Services/Identity/TradeFlow.Identity.Infrastructure/"]
COPY ["src/Services/Identity/TradeFlow.Identity.API/TradeFlow.Identity.API.csproj", "src/Services/Identity/TradeFlow.Identity.API/"]
COPY ["Directory.Build.props", "./"]
RUN dotnet restore "src/Services/Identity/TradeFlow.Identity.API/TradeFlow.Identity.API.csproj"
COPY . .
WORKDIR "/src/src/Services/Identity/TradeFlow.Identity.API"
RUN dotnet build "TradeFlow.Identity.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TradeFlow.Identity.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:80
ENTRYPOINT ["dotnet", "TradeFlow.Identity.API.dll"]
