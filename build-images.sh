#!/bin/bash

# PmPulse Docker Build Script
# This script builds all Docker images for the PmPulse application

set -e  # Exit on error

echo "================================"
echo "Building PmPulse Docker Images"
echo "================================"
echo ""

# Colors for output
GREEN='\033[0;32m'
BLUE='\033[0;34m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Check if Docker is running
if ! docker info > /dev/null 2>&1; then
    echo -e "${YELLOW}Error: Docker is not running. Please start Docker Desktop and try again.${NC}"
    exit 1
fi

echo -e "${BLUE}Step 1/2: Building Orleans Silo Host image...${NC}"
docker build -f PmPulse.FeedSiloHost/Dockerfile -t pmpulse-silohost:latest .
echo -e "${GREEN}✓ Orleans Silo Host image built successfully${NC}"
echo ""

echo -e "${BLUE}Step 2/2: Building Web API image (release)...${NC}"
docker build -f PmPulse.WebApi/Dockerfile.release -t pmpulse-webapi:latest .
echo -e "${GREEN}✓ Web API image built successfully${NC}"
echo ""

echo "================================"
echo -e "${GREEN}All images built successfully!${NC}"
echo "================================"
echo ""
echo "Available images:"
docker images | grep pmpulse
echo ""
echo "Next steps:"
echo "  1. Start services: docker-compose up"
echo "  2. Or start in background: docker-compose up -d"
echo "  3. View logs: docker-compose logs -f"
echo ""

