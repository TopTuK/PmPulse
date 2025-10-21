#!/bin/bash

# PmPulse Aspire AppHost Startup Script
# This script checks Docker and starts the Aspire AppHost with Docker containers

set -e

echo "================================"
echo "Starting PmPulse with Aspire"
echo "================================"
echo ""

# Colors for output
GREEN='\033[0;32m'
BLUE='\033[0;34m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
NC='\033[0m' # No Color

# Setup .NET SDK path
echo -e "${BLUE}Setting up .NET SDK...${NC}"
if [ -d "$HOME/.dotnet" ]; then
    export PATH="$HOME/.dotnet:$PATH"
    export DOTNET_ROOT="$HOME/.dotnet"
elif [ -d "/usr/local/share/dotnet" ]; then
    export PATH="/usr/local/share/dotnet:$PATH"
    export DOTNET_ROOT="/usr/local/share/dotnet"
fi

# Verify dotnet is available
if ! command -v dotnet &> /dev/null; then
    echo -e "${RED}❌ .NET SDK not found!${NC}"
    echo ""
    echo "Please install .NET SDK 8.0 or later from:"
    echo "  https://dotnet.microsoft.com/download"
    echo ""
    exit 1
fi

echo -e "${GREEN}✓ .NET SDK found: $(dotnet --version)${NC}"
echo ""

# Check if Docker is running
echo -e "${BLUE}Checking Docker status...${NC}"
if ! docker info > /dev/null 2>&1; then
    echo -e "${RED}❌ Docker is not running!${NC}"
    echo ""
    echo "Please start Docker Desktop and try again."
    echo ""
    echo "After Docker starts, run:"
    echo "  ./start-aspire.sh"
    echo ""
    exit 1
fi

echo -e "${GREEN}✓ Docker is running${NC}"
echo ""

# Start Aspire AppHost in Docker mode
echo -e "${BLUE}Starting Aspire AppHost with Docker containers...${NC}"
echo -e "${YELLOW}This will:${NC}"
echo "  1. Build Docker images for all services"
echo "  2. Start Redis for Orleans clustering"
echo "  3. Start Orleans Silo Host in Docker"
echo "  4. Start Web API in Docker"
echo "  5. Start Vue.js Frontend in Docker"
echo "  6. Launch Aspire Dashboard for monitoring"
echo ""
echo -e "${YELLOW}Services will be available at:${NC}"
echo "  • Aspire Dashboard: (will be shown after startup)"
echo "  • Frontend:  http://localhost:5173"
echo "  • Web API:   http://localhost:8080"
echo "  • Redis:     localhost:6379"
echo ""
echo -e "${YELLOW}Press Ctrl+C to stop all services${NC}"
echo ""

# Run the AppHost with Docker environments
export STARTUP_TYPE=docker # This triggers the Docker mode in Program.cs
dotnet run --project PmPulse.AppHost

echo ""
echo -e "${GREEN}Aspire AppHost stopped${NC}"

