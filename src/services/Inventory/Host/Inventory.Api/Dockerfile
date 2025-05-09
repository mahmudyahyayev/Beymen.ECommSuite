FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Build.targets", "."]
COPY ["nuget.config", "."]
COPY ["src/services/Inventory/Host/Inventory.Api/Inventory.Api.csproj", "src/services/Inventory/Host/Inventory.Api/"]
COPY ["src/services/Inventory/Infrastructure/Inventory.Infrastructure/Inventory.Infrastructure.csproj", "src/services/Inventory/Infrastructure/Inventory.Infrastructure/"]
COPY ["building-blokcs/BuildingBlocks.HealthCheck/BuildingBlocks.HealthCheck.csproj", "building-blokcs/BuildingBlocks.HealthCheck/"]
COPY ["building-blokcs/BuildingBlocks.Abstractions/BuildingBlocks.Abstractions.csproj", "building-blokcs/BuildingBlocks.Abstractions/"]
COPY ["building-blokcs/BuildingBlocks.Core/BuildingBlocks.Core.csproj", "building-blokcs/BuildingBlocks.Core/"]
COPY ["building-blokcs/BuildingBlocks.Integration.MassTransit/BuildingBlocks.Integration.MassTransit.csproj", "building-blokcs/BuildingBlocks.Integration.MassTransit/"]
COPY ["building-blokcs/BuildingBlocks.Messaging.Persistence.Postgres/BuildingBlocks.Messaging.Persistence.Postgres.csproj", "building-blokcs/BuildingBlocks.Messaging.Persistence.Postgres/"]
COPY ["building-blokcs/BuildingBlocks.Persistence.EfCore.Postgres/BuildingBlocks.Persistence.EfCore.Postgres.csproj", "building-blokcs/BuildingBlocks.Persistence.EfCore.Postgres/"]
COPY ["building-blokcs/BuildingBlocks.Swagger/BuildingBlocks.Swagger.csproj", "building-blokcs/BuildingBlocks.Swagger/"]
COPY ["src/services/Shared/Beymen.ECommSuite.Shared/Beymen.ECommSuite.Shared.csproj", "src/services/Shared/Beymen.ECommSuite.Shared/"]
COPY ["src/services/Inventory/Application/Inventory.Application/Inventory.Application.csproj", "src/services/Inventory/Application/Inventory.Application/"]
COPY ["building-blokcs/BuildingBlocks.Caching/BuildingBlocks.Caching.csproj", "building-blokcs/BuildingBlocks.Caching/"]
COPY ["building-blokcs/BuildingBlocks.Logging/BuildingBlocks.Logging.csproj", "building-blokcs/BuildingBlocks.Logging/"]
COPY ["building-blokcs/BuildingBlocks.Web/BuildingBlocks.Web.csproj", "building-blokcs/BuildingBlocks.Web/"]
COPY ["src/services/Inventory/Core/Inventory.Domain/Inventory.Domain.csproj", "src/services/Inventory/Core/Inventory.Domain/"]
COPY ["building-blokcs/BuildingBlocks.FluentValidation/BuildingBlocks.FluentValidation.csproj", "building-blokcs/BuildingBlocks.FluentValidation/"]
RUN dotnet restore "./src/services/Inventory/Host/Inventory.Api/Inventory.Api.csproj"
COPY . .
WORKDIR "/src/src/services/Inventory/Host/Inventory.Api"
RUN dotnet build "./Inventory.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Inventory.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Inventory.Api.dll"]