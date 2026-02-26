FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/BuildingBlocks/TradeFlow.Common/TradeFlow.Common.csproj", "src/BuildingBlocks/TradeFlow.Common/"]
COPY ["src/BuildingBlocks/TradeFlow.MessageContracts/TradeFlow.MessageContracts.csproj", "src/BuildingBlocks/TradeFlow.MessageContracts/"]
COPY ["src/Services/Social/TradeFlow.Social.Domain/TradeFlow.Social.Domain.csproj", "src/Services/Social/TradeFlow.Social.Domain/"]
COPY ["src/Services/Social/TradeFlow.Social.Application/TradeFlow.Social.Application.csproj", "src/Services/Social/TradeFlow.Social.Application/"]
COPY ["src/Services/Social/TradeFlow.Social.Infrastructure/TradeFlow.Social.Infrastructure.csproj", "src/Services/Social/TradeFlow.Social.Infrastructure/"]
COPY ["src/Services/Social/TradeFlow.Social.API/TradeFlow.Social.API.csproj", "src/Services/Social/TradeFlow.Social.API/"]
COPY ["Directory.Build.props", "./"]
RUN dotnet restore "src/Services/Social/TradeFlow.Social.API/TradeFlow.Social.API.csproj"
COPY . .
WORKDIR "/src/src/Services/Social/TradeFlow.Social.API"
RUN dotnet build "TradeFlow.Social.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TradeFlow.Social.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:80
ENTRYPOINT ["dotnet", "TradeFlow.Social.API.dll"]
