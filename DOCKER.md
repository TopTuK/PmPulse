# Docker Deployment Guide for PmPulse

This guide provides detailed instructions for running PmPulse using Docker containers.

## Quick Start

### Option 1: .NET Aspire with Docker (Recommended)

**Using the startup scripts:**
```bash
# Make sure Docker is running
./run-apphost.sh          # macOS/Linux
# or
.\run-apphost.ps1         # Windows PowerShell
```

**Or directly:**
```bash
dotnet run --project PmPulse.AppHost
```

Aspire will automatically:
- Build Docker images for all services
- Start containers with proper networking
- Configure Orleans clustering
- Launch the Aspire Dashboard for monitoring

### Option 2: Docker Compose

```bash
# Build and start all services
docker-compose up --build

# Or run in detached mode
docker-compose up -d --build

# View logs
docker-compose logs -f

# Stop all services
docker-compose down
```

## Container Architecture

```
┌─────────────────────────────────────────┐
│         Docker Network                   │
│         (pmpulse-network)                │
│                                          │
│  ┌──────────────┐                       │
│  │   Frontend   │  Port 80              │
│  │   (Nginx)    │◄──────────────┐       │
│  └──────┬───────┘               │       │
│         │                        │       │
│         ▼                        │       │
│  ┌──────────────┐                │       │
│  │   Web API    │  Port 8080     │       │
│  │   (.NET)     │◄───────────────┘       │
│  └──────┬───────┘                        │
│         │                                 │
│         ▼                                 │
│  ┌──────────────┐                        │
│  │ Orleans Silo │  Ports 11111, 30000    │
│  │   Host       │                        │
│  └──────────────┘                        │
│                                          │
└─────────────────────────────────────────┘
```

## Service Details

### 1. Orleans Silo Host (pmpulse-silohost)

**Purpose:** Hosts Orleans grains for distributed feed processing

**Image:** Built from `PmPulse.FeedSiloHost/Dockerfile`

**Ports:**
- `11111`: Silo-to-silo communication
- `30000`: Client gateway port

**Environment Variables:**
```bash
ASPNETCORE_ENVIRONMENT=Development
DOTNET_RUNNING_IN_CONTAINER=true
ORLEANS_SILO_PORT=11111
ORLEANS_GATEWAY_PORT=30000
ORLEANS_ADVERTISED_IP=pmpulse-silohost
```

### 2. Web API (pmpulse-webapi)

**Purpose:** RESTful API for frontend communication

**Image:** Built from `PmPulse.WebApi/Dockerfile`

**Ports:**
- `8080`: HTTP endpoint

**Environment Variables:**
```bash
ASPNETCORE_ENVIRONMENT=Development
DOTNET_RUNNING_IN_CONTAINER=true
ASPNETCORE_URLS=http://+:8080
ORLEANS_SILO_HOST=pmpulse-silohost
ORLEANS_GATEWAY_PORT=30000
```

**Dependencies:** Requires Orleans Silo to be running

### 3. Frontend (front)

**Purpose:** Vue.js application served by Nginx

**Image:** Built from `webapp/Dockerfile`

**Ports:**
- `80`: HTTP web server

**Dependencies:** Requires Web API to be running

## Manual Docker Commands

### Building Images

```bash
# Build all images from root directory
docker build -f PmPulse.FeedSiloHost/Dockerfile -t pmpulse-silohost:latest .
docker build -f PmPulse.WebApi/Dockerfile -t pmpulse-webapi:latest .
docker build -f webapp/Dockerfile -t pmpulse-front:latest ./webapp
```

### Running Containers Manually

```bash
# Create a network
docker network create pmpulse-network

# Run Orleans Silo
docker run -d \
  --name pmpulse-silohost \
  --network pmpulse-network \
  -p 11111:11111 \
  -p 30000:30000 \
  -e ASPNETCORE_ENVIRONMENT=Development \
  -e DOTNET_RUNNING_IN_CONTAINER=true \
  -e ORLEANS_SILO_PORT=11111 \
  -e ORLEANS_GATEWAY_PORT=30000 \
  -e ORLEANS_ADVERTISED_IP=pmpulse-silohost \
  pmpulse-silohost:latest

# Run Web API
docker run -d \
  --name pmpulse-webapi \
  --network pmpulse-network \
  -p 8080:8080 \
  -e ASPNETCORE_ENVIRONMENT=Development \
  -e DOTNET_RUNNING_IN_CONTAINER=true \
  -e ASPNETCORE_URLS=http://+:8080 \
  -e ORLEANS_SILO_HOST=pmpulse-silohost \
  -e ORLEANS_GATEWAY_PORT=30000 \
  pmpulse-webapi:latest

# Run Frontend
docker run -d \
  --name pmpulse-front \
  --network pmpulse-network \
  -p 80:80 \
  pmpulse-front:latest
```

### Cleanup

```bash
# Stop and remove all containers
docker stop pmpulse-front pmpulse-webapi pmpulse-silohost
docker rm pmpulse-front pmpulse-webapi pmpulse-silohost

# Remove network
docker network rm pmpulse-network

# Remove images
docker rmi pmpulse-front:latest pmpulse-webapi:latest pmpulse-silohost:latest
```

## Troubleshooting

### Check Container Status

```bash
# View running containers
docker ps

# View all containers (including stopped)
docker ps -a

# View container logs
docker logs pmpulse-silohost
docker logs pmpulse-webapi
docker logs pmpulse-front

# Follow logs in real-time
docker logs -f pmpulse-webapi
```

### Common Issues

#### 1. Orleans Connection Failed

**Symptom:** Web API can't connect to Orleans Silo

**Solution:** 
- Ensure silo is fully started before web API: `docker logs pmpulse-silohost`
- Check network connectivity: `docker network inspect pmpulse-network`
- Verify environment variables are set correctly

#### 2. Frontend Can't Reach API

**Symptom:** Frontend shows connection errors

**Solution:**
- Verify Web API is running: `docker ps | grep pmpulse-webapi`
- Check API logs: `docker logs pmpulse-webapi`
- Ensure API is listening on correct port (8080)

#### 3. Port Already in Use

**Symptom:** Container fails to start with port binding error

**Solution:**
```bash
# Check what's using the port (example for port 8080)
lsof -i :8080  # On macOS/Linux
netstat -ano | findstr :8080  # On Windows

# Stop the conflicting service or change the port in docker-compose.yml
```

#### 4. Build Failures

**Symptom:** Docker build fails

**Solution:**
- Ensure .dockerignore is in place
- Clear Docker build cache: `docker builder prune`
- Check that all dependencies are available
- Verify you're running from the correct directory

### Inspecting Containers

```bash
# Execute commands inside a running container
docker exec -it pmpulse-webapi /bin/bash

# Inspect container configuration
docker inspect pmpulse-silohost

# View resource usage
docker stats
```

## Production Considerations

### Security

1. **Remove Development Environment Variables:**
   ```bash
   ASPNETCORE_ENVIRONMENT=Production
   ```

2. **Use Secrets Management:**
   - Don't hardcode sensitive data in docker-compose.yml
   - Use Docker secrets or external secret management

3. **Enable HTTPS:**
   - Configure SSL certificates
   - Update ASPNETCORE_URLS to include https

### Persistence

For production, consider:

1. **Orleans Storage:**
   - Replace in-memory storage with persistent storage (e.g., Azure Storage, SQL Server)
   - Add volume mounts for data persistence

2. **Configuration:**
   - Use external configuration sources
   - Mount appsettings.Production.json as volumes

### Scaling

```bash
# Scale Web API instances
docker-compose up -d --scale pmpulse-webapi=3

# Note: Orleans Silo scaling requires additional configuration
# for cluster membership (e.g., using Azure Table Storage or SQL Server)
```

### Health Checks

Add health checks to docker-compose.yml:

```yaml
services:
  pmpulse-webapi:
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:8080/health"]
      interval: 30s
      timeout: 10s
      retries: 3
      start_period: 40s
```

## Additional Resources

- [.NET Aspire Documentation](https://learn.microsoft.com/en-us/dotnet/aspire/)
- [Orleans Documentation](https://docs.microsoft.com/en-us/dotnet/orleans/)
- [Docker Documentation](https://docs.docker.com/)
- [Docker Compose Documentation](https://docs.docker.com/compose/)

## Support

For issues related to Docker deployment, please check:
1. Container logs: `docker logs <container-name>`
2. Network connectivity: `docker network inspect pmpulse-network`
3. Environment variables: `docker inspect <container-name>`

---

**Happy Containerizing! 🐳**

