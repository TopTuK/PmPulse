# PmPulse Docker Build Script (PowerShell)
# This script builds all Docker images for the PmPulse application

$ErrorActionPreference = "Stop"

Write-Host "================================" -ForegroundColor Cyan
Write-Host "Building PmPulse Docker Images" -ForegroundColor Cyan
Write-Host "================================" -ForegroundColor Cyan
Write-Host ""

# Check if Docker is running
try {
    docker info | Out-Null
} catch {
    Write-Host "Error: Docker is not running. Please start Docker Desktop and try again." -ForegroundColor Yellow
    exit 1
}

Write-Host "Step 1/3: Building Orleans Silo Host image..." -ForegroundColor Blue
docker build -f PmPulse.FeedSiloHost/Dockerfile -t pmpulse-silohost:latest .
Write-Host "✓ Orleans Silo Host image built successfully" -ForegroundColor Green
Write-Host ""

Write-Host "Step 2/3: Building Web API image..." -ForegroundColor Blue
docker build -f PmPulse.WebApi/Dockerfile -t pmpulse-webapi:latest .
Write-Host "✓ Web API image built successfully" -ForegroundColor Green
Write-Host ""

Write-Host "Step 3/3: Building Frontend image..." -ForegroundColor Blue
docker build -f webapp/Dockerfile -t pmpulse-front:latest ./webapp
Write-Host "✓ Frontend image built successfully" -ForegroundColor Green
Write-Host ""

Write-Host "================================" -ForegroundColor Cyan
Write-Host "All images built successfully!" -ForegroundColor Green
Write-Host "================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Available images:"
docker images | Select-String "pmpulse"
Write-Host ""
Write-Host "Next steps:"
Write-Host "  1. Start services: docker-compose up"
Write-Host "  2. Or start in background: docker-compose up -d"
Write-Host "  3. View logs: docker-compose logs -f"
Write-Host ""

