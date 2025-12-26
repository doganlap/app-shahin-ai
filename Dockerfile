# Multi-stage build for GRC Platform API
# Optimized for Railway deployment

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /source

# Copy solution and all source files
COPY aspnet-core/ ./

# Restore and build
WORKDIR /source
RUN dotnet restore Grc.sln

# Publish the API project
WORKDIR /source/src/Grc.HttpApi.Host
RUN dotnet publish -c Release -o /app/publish --no-restore

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

# Copy published application
COPY --from=build /app/publish .

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Railway sets PORT environment variable
ENV ASPNETCORE_URLS=http://0.0.0.0:${PORT:-5000}
ENV ASPNETCORE_ENVIRONMENT=Production

# Expose port (Railway will override with $PORT)
EXPOSE ${PORT:-5000}

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=60s --retries=3 \
  CMD curl -f http://localhost:${PORT:-5000}/health || exit 1

# Start the application
ENTRYPOINT ["dotnet", "Grc.HttpApi.Host.dll"]
