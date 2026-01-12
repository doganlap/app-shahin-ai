#!/bin/bash
# Unified GRC Application Startup Script
# All code runs from ONE source directory: aspnet-core

export ASPNETCORE_ENVIRONMENT=Production
export ASPNETCORE_URLS="http://0.0.0.0:5500;https://0.0.0.0:5501"

cd /root/app.shahin-ai.com/Shahin-ai/aspnet-core/src/Grc.Web

echo "Starting GRC Web UI from unified source location..."
echo "Source: $(pwd)"
echo "Environment: $ASPNETCORE_ENVIRONMENT"
echo "URLs: $ASPNETCORE_URLS"

dotnet run --no-launch-profile
