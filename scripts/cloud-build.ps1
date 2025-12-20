# ============================================================
# Cloud Server Build Script for ABP.io GRC Platform (PowerShell)
# ============================================================

$ErrorActionPreference = "Stop"

Write-Host "==========================================" -ForegroundColor Green
Write-Host "ABP.io GRC Platform - Cloud Build" -ForegroundColor Green
Write-Host "==========================================" -ForegroundColor Green

# Configuration
$ProjectDir = $env:PROJECT_DIR
if (-not $ProjectDir) {
    $ProjectDir = "C:\grc-platform"
}

# Step 1: Check .NET SDK
Write-Host "`nStep 1: Checking .NET SDK..." -ForegroundColor Yellow
if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    Write-Host "ERROR: .NET 8.0 SDK not found. Please install it first." -ForegroundColor Red
    Write-Host "Download from: https://dotnet.microsoft.com/download/dotnet/8.0" -ForegroundColor Yellow
    exit 1
}
$dotnetVersion = dotnet --version
Write-Host "Found .NET SDK version: $dotnetVersion" -ForegroundColor Green

# Step 2: Install EF Core tools
Write-Host "`nStep 2: Installing EF Core tools..." -ForegroundColor Yellow
dotnet tool install --global dotnet-ef --version 8.0.0
if ($LASTEXITCODE -ne 0) {
    dotnet tool update --global dotnet-ef
}

# Step 3: Navigate to project directory
Write-Host "`nStep 3: Navigating to project directory..." -ForegroundColor Yellow
if (-not (Test-Path $ProjectDir)) {
    Write-Host "ERROR: Project directory not found: $ProjectDir" -ForegroundColor Red
    exit 1
}
Set-Location $ProjectDir
Write-Host "Current directory: $(Get-Location)" -ForegroundColor Green

# Step 4: Restore packages
Write-Host "`nStep 4: Restoring NuGet packages..." -ForegroundColor Yellow
dotnet restore
if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: Package restore failed" -ForegroundColor Red
    exit 1
}

# Step 5: Build solution
Write-Host "`nStep 5: Building solution..." -ForegroundColor Yellow
dotnet build --configuration Release --no-restore
if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: Build failed" -ForegroundColor Red
    exit 1
}

# Step 6: Run database migrations
Write-Host "`nStep 6: Applying database migrations..." -ForegroundColor Yellow
$connectionString = $env:ConnectionStrings__Default
if (-not $connectionString) {
    Write-Host "WARNING: ConnectionStrings__Default not set. Using default PostgreSQL connection." -ForegroundColor Yellow
    $connectionString = "Host=localhost;Database=grc_db;Username=postgres;Password=postgres"
}

Set-Location "src\Grc.EntityFrameworkCore"
$env:ConnectionStrings__Default = $connectionString
dotnet ef database update --startup-project "..\..\Grc.HttpApi.Host"
Set-Location $ProjectDir

# Step 7: Publish application
Write-Host "`nStep 7: Publishing application..." -ForegroundColor Yellow
$publishDir = Join-Path $ProjectDir "publish"
dotnet publish "src\Grc.HttpApi.Host\Grc.HttpApi.Host.csproj" `
    --configuration Release `
    --output $publishDir `
    --no-build

Write-Host "`n==========================================" -ForegroundColor Green
Write-Host "Build complete!" -ForegroundColor Green
Write-Host "==========================================" -ForegroundColor Green
Write-Host "Published to: $publishDir" -ForegroundColor Green
Write-Host "`nTo run the application:" -ForegroundColor Yellow
Write-Host "  cd $publishDir" -ForegroundColor Cyan
Write-Host "  dotnet Grc.HttpApi.Host.dll" -ForegroundColor Cyan


