#!/bin/bash

# Build Production Script for PmPulse
# This script builds all Docker images defined in docker-compose.yml

set -e  # Exit on any error

echo "🚀 Building PmPulse Production Images..."

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Function to print colored output
print_status() {
    echo -e "${GREEN}[INFO]${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}[WARN]${NC} $1"
}

print_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Check if docker-compose.yml exists
if [ ! -f "docker-compose.yml" ]; then
    print_error "docker-compose.yml not found in current directory"
    exit 1
fi

# Check if Docker is running
if ! docker info > /dev/null 2>&1; then
    print_error "Docker is not running. Please start Docker and try again."
    exit 1
fi

print_status "Building all services from docker-compose.yml..."

# Build all services without cache to ensure fresh builds
docker-compose build --no-cache

if [ $? -eq 0 ]; then
    print_status "✅ All images built successfully!"
    
    # Show built images
    print_status "Built images:"
    docker images | grep -E "(pmpulse-silohost|pmpulse-webapi)" || true
    
    print_status "🎉 Production build completed!"
else
    print_error "❌ Build failed!"
    exit 1
fi
