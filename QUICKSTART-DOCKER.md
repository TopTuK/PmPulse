# Quick Start - Docker Deployment

This guide will get you up and running with PmPulse using Docker in just a few minutes.

## Prerequisites

✅ **Docker Desktop** - Make sure it's installed and **running**

Check if Docker is running:
```bash
docker --version
docker info
```

## 🚀 Quick Start (3 Steps)

### Step 1: Start Docker Desktop

Make sure Docker Desktop is running on your system.

### Step 2: Build Images

Choose your platform:

**macOS/Linux:**
```bash
./build-images.sh
```

**Windows (PowerShell):**
```powershell
.\build-images.ps1
```

**Or manually:**
```bash
docker build -f PmPulse.FeedSiloHost/Dockerfile -t pmpulse-silohost:latest .
docker build -f PmPulse.WebApi/Dockerfile -t pmpulse-webapi:latest .
docker build -f webapp/Dockerfile -t pmpulse-front:latest ./webapp
```

### Step 3: Start All Services

```bash
docker-compose up
```

Or run in background (detached mode):
```bash
docker-compose up -d
```

## 🎯 Access Your Application

Once all services are running:

- **Frontend (Web UI)**: http://localhost:80
- **Web API**: http://localhost:8080
- **Orleans Silo**: Ports 11111 (silo), 30000 (gateway)

## 📊 Monitoring

### View Logs

```bash
# All services
docker-compose logs -f

# Specific service
docker-compose logs -f pmpulse-webapi
docker-compose logs -f pmpulse-silohost
docker-compose logs -f front
```

### Check Container Status

```bash
# List running containers
docker-compose ps

# Check health status
docker ps --format "table {{.Names}}\t{{.Status}}"
```

### Service Health Checks

The docker-compose configuration includes automatic health checks:

- **Orleans Silo**: Checks port 11111 connectivity
- **Web API**: Checks `/FeedBlock/GetAllFeedBlocks` endpoint
- **Frontend**: Checks Nginx is serving on port 80

Services will wait for dependencies to be healthy before starting.

## 🛠 Common Commands

### Stop Services
```bash
docker-compose stop
```

### Start Services (already built)
```bash
docker-compose start
```

### Restart Services
```bash
docker-compose restart
```

### Stop and Remove Containers
```bash
docker-compose down
```

### Rebuild and Restart
```bash
docker-compose up --build
```

### Remove Everything (including volumes)
```bash
docker-compose down -v
```

## 🔍 Troubleshooting

### Docker Not Running

**Error:** `Cannot connect to the Docker daemon`

**Solution:** Start Docker Desktop application

### Port Already in Use

**Error:** `Bind for 0.0.0.0:8080 failed: port is already allocated`

**Solution:** Stop the process using that port or change the port in `docker-compose.yml`:
```yaml
ports:
  - "8081:8080"  # Change 8080 to 8081
```

### Build Failures

**Error:** Various build errors

**Solutions:**
1. Clear Docker cache: `docker builder prune -a`
2. Ensure you're in the project root directory
3. Check Docker has enough resources (Settings → Resources)

### Container Keeps Restarting

Check the logs:
```bash
docker-compose logs pmpulse-silohost
```

Common issues:
- Orleans Silo not ready - wait 40s for startup
- Web API can't connect to Silo - check network configuration
- Frontend build failed - check Node.js dependencies

### Can't Access Services

1. Verify containers are running:
   ```bash
   docker-compose ps
   ```

2. Check if all services are healthy:
   ```bash
   docker ps
   ```
   Look for "healthy" status

3. Check port bindings:
   ```bash
   docker-compose port front 80
   docker-compose port pmpulse-webapi 8080
   ```

## 🔧 Advanced Usage

### Scale Services

```bash
# Run multiple Web API instances
docker-compose up -d --scale pmpulse-webapi=3
```

Note: Orleans Silo scaling requires additional configuration.

### Access Container Shell

```bash
# Access Web API container
docker exec -it pmpulse-webapi /bin/bash

# Access Frontend container
docker exec -it pmpulse-front /bin/sh
```

### Inspect Container

```bash
docker inspect pmpulse-silohost
docker inspect pmpulse-webapi
docker inspect pmpulse-front
```

### View Resource Usage

```bash
docker stats
```

## 📝 Configuration

### Environment Variables

Edit `docker-compose.yml` to customize:

```yaml
environment:
  - ASPNETCORE_ENVIRONMENT=Production  # Change to Production
  - ORLEANS_SILO_PORT=11111           # Change silo port
  - ORLEANS_GATEWAY_PORT=30000        # Change gateway port
```

### Port Mapping

Change exposed ports in `docker-compose.yml`:

```yaml
ports:
  - "8080:8080"  # Format: HOST:CONTAINER
```

## 🚨 Emergency Commands

### Stop All Containers Immediately
```bash
docker stop $(docker ps -q)
```

### Remove All PmPulse Containers
```bash
docker rm -f pmpulse-silohost pmpulse-webapi pmpulse-front
```

### Remove All PmPulse Images
```bash
docker rmi -f pmpulse-silohost:latest pmpulse-webapi:latest pmpulse-front:latest
```

### Complete Reset
```bash
# Stop and remove everything
docker-compose down -v

# Remove images
docker rmi -f pmpulse-silohost:latest pmpulse-webapi:latest pmpulse-front:latest

# Rebuild from scratch
./build-images.sh
docker-compose up
```

## 📚 Next Steps

- Read [DOCKER.md](DOCKER.md) for detailed Docker documentation
- See [README.md](README.md) for application features and architecture
- Check the [Aspire Dashboard](#using-aspire) for advanced monitoring

## 🎓 Using Aspire

For the best development experience with .NET Aspire orchestration:

**Using the startup scripts:**
```bash
./run-apphost.sh          # macOS/Linux
# or
.\run-apphost.ps1         # Windows PowerShell
```

**Or directly:**
```bash
dotnet run --project PmPulse.AppHost
```

This provides:
- Automatic Docker container management
- Built-in dashboard for monitoring
- Distributed tracing
- Better developer experience

---

**Questions?** See [DOCKER.md](DOCKER.md) for comprehensive documentation.

