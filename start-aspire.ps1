# PmPulse Aspire AppHost Startup Script (PowerShell)
# This script checks Docker and starts the Aspire AppHost with Docker containers

$ErrorActionPreference = "Stop"

Write-Host "================================" -ForegroundColor Cyan
Write-Host "Starting PmPulse with Aspire" -ForegroundColor Cyan
Write-Host "================================" -ForegroundColor Cyan
Write-Host ""

# Setup .NET SDK path
Write-Host "Setting up .NET SDK..." -ForegroundColor Blue
$dotnetPaths = @(
    "$env:USERPROFILE\.dotnet",
    "C:\Program Files\dotnet",
    "C:\Program Files (x86)\dotnet"
)

foreach ($path in $dotnetPaths) {
    if (Test-Path $path) {
        $env:PATH = "$path;$env:PATH"
        $env:DOTNET_ROOT = $path
        break
    }
}

# Verify dotnet is available
try {
    $dotnetVersion = & dotnet --version
    Write-Host "✓ .NET SDK found: $dotnetVersion" -ForegroundColor Green
    Write-Host ""
} catch {
    Write-Host "❌ .NET SDK not found!" -ForegroundColor Red
    Write-Host ""
    Write-Host "Please install .NET SDK 8.0 or later from:"
    Write-Host "  https://dotnet.microsoft.com/download"
    Write-Host ""
    exit 1
}

# Check if Docker is running
Write-Host "Checking Docker status..." -ForegroundColor Blue
try {
    docker info | Out-Null
    Write-Host "✓ Docker is running" -ForegroundColor Green
    Write-Host ""
} catch {
    Write-Host "❌ Docker is not running!" -ForegroundColor Red
    Write-Host ""
    Write-Host "Please start Docker Desktop and try again."
    Write-Host ""
    Write-Host "After Docker starts, run:"
    Write-Host "  .\start-aspire.ps1"
    Write-Host ""
    exit 1
}

# Start Aspire AppHost
Write-Host "Starting Aspire AppHost..." -ForegroundColor Blue
Write-Host "This will:" -ForegroundColor Yellow
Write-Host "  1. Build Docker images for all services"
Write-Host "  2. Start containers with proper orchestration"
Write-Host "  3. Launch Aspire Dashboard for monitoring"
Write-Host ""
Write-Host "Press Ctrl+C to stop all services" -ForegroundColor Yellow
Write-Host ""

# Run the AppHost
dotnet run --project PmPulse.AppHost

Write-Host ""
Write-Host "Aspire AppHost stopped" -ForegroundColor Green

