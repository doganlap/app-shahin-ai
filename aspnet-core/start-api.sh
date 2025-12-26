#!/bin/bash

# Saudi GRC Platform - API Startup Script
# This script sets environment variables and starts the API server

# Load environment variables from .env file
if [ -f .env ]; then
    export $(cat .env | grep -v '^#' | xargs)
fi

# Set ASP.NET Core environment
export ASPNETCORE_ENVIRONMENT=Production
export ASPNETCORE_URLS="http://localhost:5000;https://localhost:5001"

# Navigate to API host directory
cd src/Grc.HttpApi.Host

echo "========================================="
echo "Saudi GRC Platform - API Server"
echo "========================================="
echo "Environment: $ASPNETCORE_ENVIRONMENT"
echo "URLs: $ASPNETCORE_URLS"
echo "========================================="
echo ""

# Run the API
dotnet run --no-build --configuration Release
