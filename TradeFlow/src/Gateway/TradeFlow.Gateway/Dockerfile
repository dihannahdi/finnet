FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/Gateway/TradeFlow.Gateway/TradeFlow.Gateway.csproj", "src/Gateway/TradeFlow.Gateway/"]
COPY ["Directory.Build.props", "./"]
RUN dotnet restore "src/Gateway/TradeFlow.Gateway/TradeFlow.Gateway.csproj"
COPY . .
WORKDIR "/src/src/Gateway/TradeFlow.Gateway"
RUN dotnet build "TradeFlow.Gateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TradeFlow.Gateway.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:80
ENTRYPOINT ["dotnet", "TradeFlow.Gateway.dll"]
