# Stage 1: Build & Publish
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy Solution & Directory configs for optimal layer caching
COPY OutcomeHub.slnx Directory.Build.props Directory.Packages.props global.json ./
COPY src/OutcomeHub.Domain/OutcomeHub.Domain.csproj src/OutcomeHub.Domain/
COPY src/OutcomeHub.Application/OutcomeHub.Application.csproj src/OutcomeHub.Application/
COPY src/OutcomeHub.Infrastructure/OutcomeHub.Infrastructure.csproj src/OutcomeHub.Infrastructure/
COPY src/OutcomeHub.Api/OutcomeHub.Api.csproj src/OutcomeHub.Api/
COPY src/OutcomeHub.Migrations/OutcomeHub.Migrations.csproj src/OutcomeHub.Migrations/

RUN dotnet restore src/OutcomeHub.Api/OutcomeHub.Api.csproj

# Copy all source code
COPY src/ src/

# Build & Publish Release
WORKDIR /src/src/OutcomeHub.Api
RUN dotnet publish OutcomeHub.Api.csproj -c Release -o /app/publish --no-restore /p:UseAppHost=false

# Stage 2: Runtime Image
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Create non-root user for security
RUN groupadd -g 1000 appgroup && \
    useradd -u 1000 -g appgroup -s /bin/bash -m appuser

COPY --from=build --chown=appuser:appgroup /app/publish .

USER appuser
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV DOTNET_EnableDiagnostics=0

ENTRYPOINT ["dotnet", "OutcomeHub.Api.dll"]
