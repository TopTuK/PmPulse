# PmPulse

A modern web service for aggregating, parsing, and summarizing content from multiple RSS feeds and Telegram channels in one unified interface.

🌐 **Live Website**: [https://news.pmi.moscow](https://news.pmi.moscow)

## Overview

PmPulse helps you stay informed without the noise. Instead of subscribing to hundreds of media accounts and dealing with constant notifications, PmPulse brings all your news sources together in a clean, organized interface.

### The Problem

- Information overload from multiple news sources
- Constant notifications from various platforms
- Being trapped in information bubbles due to algorithmic feeds
- Difficulty discovering quality content outside your usual sources

### The Solution

PmPulse aggregates content from diverse sources and presents it through customizable thematic and people-based collections. This approach helps you:

- Track news from different areas in one place
- Discover new sources of information through curated collections
- Break out of information bubbles with diverse perspectives
- Stay informed without notification fatigue

## Features

- **Multi-Source Aggregation**: Combine RSS feeds and Telegram channels in one interface
- **Smart Parsing**: Advanced parsing capabilities for various content formats
- **Content Summarization**: Automatic summarization of articles for quick scanning
- **Thematic Collections**: Organize feeds by topic or theme
- **People-Based Collections**: Follow curated collections from trusted individuals
- **Modern UI**: Clean, responsive interface built with Vue.js and Tailwind CSS
- **Scalable Architecture**: Built on Orleans for distributed processing

## Technology Stack

### Backend
- **ASP.NET Core**: Web API framework
- **Orleans**: Distributed virtual actor framework for scalable feed processing
- **.NET Aspire**: Cloud-ready app orchestration
- **C# 12**: Modern language features

### Frontend
- **Vue.js 3**: Progressive JavaScript framework
- **Vite**: Fast build tool and dev server
- **Tailwind CSS 4**: Utility-first CSS framework
- **Vuestic UI**: Vue 3 component library
- **Pinia**: State management
- **Vue Router**: Client-side routing
- **Vue I18n**: Internationalization support

### Infrastructure
- **Orleans Silo**: Distributed grain hosting
- **Service Defaults**: Shared service configurations
- **Docker**: Containerization support for all services
- **Docker Compose**: Multi-container orchestration

## Architecture

PmPulse uses a distributed grain-based architecture powered by Microsoft Orleans:

```
┌─────────────────┐
│   Vue.js App    │  ← Frontend (webapp/)
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│   Web API       │  ← RESTful API (PmPulse.WebApi/)
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│  Orleans Silo   │  ← Grain Host (PmPulse.FeedSiloHost/)
└────────┬────────┘
         │
         ▼
┌─────────────────────────────────┐
│  Grains (Virtual Actors)        │
│  • FeedGrain                    │  ← Feed management
│  • RssFeedFetcherGrain          │  ← RSS parsing
│  • TelegramFeedFetcherGrain     │  ← Telegram parsing
└─────────────────────────────────┘
```

### Key Components

- **Feed Grains**: Manage individual feed state and lifecycle
- **Fetcher Grains**: Handle content retrieval and parsing for different source types
- **Block Service**: Organize feeds into thematic blocks
- **Feed Service**: Coordinate feed operations

## Getting Started

### Prerequisites

#### For Docker Deployment (Recommended)
- **Docker Desktop** or **Docker Engine** with Docker Compose
- **.NET 8.0 SDK** (for Aspire orchestration)

#### For Local Development
- **.NET 8.0 SDK** or later
- **Node.js 20.19+** or **22.12+**
- **npm** or **yarn**

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/yourusername/PmPulse.git
   cd PmPulse
   ```

2. **For Docker deployment (skip to step 3 for local development)**
   
   See [QUICKSTART-DOCKER.md](QUICKSTART-DOCKER.md) for quick Docker setup, or:
   
   ```bash
   # Build images
   ./build-images.sh          # macOS/Linux
   # or
   .\build-images.ps1         # Windows PowerShell
   
   # Start services
   docker-compose up
   ```

3. **For local development**
   
   **Restore .NET dependencies:**
   ```bash
   dotnet restore
   ```

   **Install frontend dependencies:**
   ```bash
   cd webapp
   npm install
   cd ..
   ```

### Running the Application

#### Option 1: Using .NET Aspire with Docker (Recommended)

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

This will start all services in Docker containers with proper orchestration. The Aspire dashboard will be available to monitor your services. All services will run in isolated containers with proper networking.

**Services will be available at:**
- Frontend: `http://localhost:80`
- Web API: `http://localhost:8080`
- Orleans Silo: Ports `11111` (silo) and `30000` (gateway)
- Aspire Dashboard: Check console output for the dashboard URL

#### Option 2: Using Docker Compose

```bash
docker-compose up --build
```

This will build and start all services using Docker Compose. Use this option if you prefer Docker Compose over Aspire orchestration.

**Services will be available at:**
- Frontend: `http://localhost:80`
- Web API: `http://localhost:8080`

To stop the services:
```bash
docker-compose down
```

#### Option 3: Local Development (No Docker)

1. **Start the Orleans Silo**
   ```bash
   dotnet run --project PmPulse.FeedSiloHost
   ```

2. **Start the Web API** (in a new terminal)
   ```bash
   dotnet run --project PmPulse.WebApi
   ```

3. **Start the frontend** (in a new terminal)
   ```bash
   cd webapp
   npm run dev
   ```

The application will be available at:
- Frontend: `http://localhost:5173`
- API: `http://localhost:5066`

## Project Structure

```
PmPulse/
├── PmPulse.AppDomain/          # Domain models and services
│   ├── Models/                 # Domain entities
│   └── Services/               # Parsing services
├── PmPulse.AppHost/            # .NET Aspire orchestration
├── PmPulse.FeedSiloHost/       # Orleans silo host
├── PmPulse.GrainClasses/       # Orleans grain implementations
│   ├── Fetchers/               # Feed fetcher grains
│   └── State/                  # Grain state models
├── PmPulse.GrainInterfaces/    # Grain interfaces
├── PmPulse.ServiceDefaults/    # Shared service configuration
├── PmPulse.WebApi/             # REST API
│   ├── Controllers/            # API endpoints
│   └── Services/               # Business logic
└── webapp/                     # Vue.js frontend
    ├── src/
    │   ├── components/         # Vue components
    │   ├── views/              # Page views
    │   ├── services/           # API clients
    │   └── stores/             # State management
    └── public/                 # Static assets
```

## Development

### Backend Development

The backend uses Orleans grains for distributed processing. To add a new feed source type:

1. Create a fetcher grain interface in `PmPulse.GrainInterfaces/`
2. Implement the grain in `PmPulse.GrainClasses/Fetchers/`
3. Add parsing logic in `PmPulse.AppDomain/Services/`
4. Register the grain in the silo configuration

### Frontend Development

```bash
cd webapp
npm run dev      # Start dev server with hot reload
npm run build    # Build for production
npm run lint     # Lint and fix code
```

The frontend uses:
- **Composition API** for component logic
- **Pinia** for state management
- **Tailwind CSS** for styling
- **Axios** for API communication

## Docker Support

PmPulse includes full Docker support with containerized deployment options:

### Docker Files

- **`PmPulse.WebApi/Dockerfile`**: Multi-stage build for the Web API
- **`PmPulse.FeedSiloHost/Dockerfile`**: Multi-stage build for the Orleans Silo
- **`webapp/Dockerfile`**: Multi-stage build with Nginx for the frontend
- **`docker-compose.yml`**: Orchestration file for all services
- **`.dockerignore`**: Optimizes Docker build context

### Container Networking

The application uses automatic service discovery in Docker:

- **Orleans Silo Host**: Advertises itself as `pmpulse-silohost` on ports 11111 (silo) and 30000 (gateway)
- **Web API**: Connects to the silo using the service name `pmpulse-silohost`
- **Frontend**: Nginx-based container serving the built Vue.js application

### Environment Variables

The following environment variables control Docker networking:

**FeedSiloHost:**
- `ORLEANS_SILO_PORT`: Silo communication port (default: 11111)
- `ORLEANS_GATEWAY_PORT`: Gateway port for clients (default: 30000)
- `ORLEANS_ADVERTISED_IP`: Service name for discovery (default: pmpulse-silohost)
- `DOTNET_RUNNING_IN_CONTAINER`: Enables Docker mode (set to "true")

**WebApi:**
- `ORLEANS_SILO_HOST`: Hostname of the Orleans Silo (default: pmpulse-silohost)
- `ORLEANS_GATEWAY_PORT`: Port to connect to gateway (default: 30000)
- `DOTNET_RUNNING_IN_CONTAINER`: Enables Docker mode (set to "true")

### Building Individual Containers

```bash
# Build Orleans Silo
docker build -f PmPulse.FeedSiloHost/Dockerfile -t pmpulse-silohost .

# Build Web API
docker build -f PmPulse.WebApi/Dockerfile -t pmpulse-webapi .

# Build Frontend
docker build -f webapp/Dockerfile -t pmpulse-front ./webapp
```

## Configuration

Configuration files:
- `PmPulse.WebApi/appsettings.json`: API configuration
- `PmPulse.AppHost/appsettings.json`: Aspire orchestration settings
- `webapp/vite.config.js`: Frontend build configuration
- `docker-compose.yml`: Docker orchestration configuration

## API Endpoints

- `GET /FeedBlock/GetAllFeedBlocks`: Get all feed blocks
- `GET /FeedBlock/GetFeedBlock?slug={slug}`: Get specific feed block by slug
- `GET /FeedPost/GetBlockFeedPosts?slug={slug}`: Get feed posts for a block
- `GET /FeedPost/GetFeedPosts?slug={slug}`: Get feed posts for a specific feed
- Additional endpoints in `PmPulse.WebApi/Controllers/`

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the [LICENSE.txt](LICENSE.txt) file for details.

## Acknowledgments

- Built with [Microsoft Orleans](https://dotnet.github.io/orleans/) for distributed systems
- UI powered by [Vuestic UI](https://vuestic.dev/)
- Styled with [Tailwind CSS](https://tailwindcss.com/)

---

**Stay informed. Stay curious. Break the bubble.**
