# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["PmPulse.FeedSiloHost/PmPulse.FeedSiloHost.csproj", "PmPulse.FeedSiloHost/"]
COPY ["PmPulse.ServiceDefaults/PmPulse.ServiceDefaults.csproj", "PmPulse.ServiceDefaults/"]
COPY ["PmPulse.AppDomain/PmPulse.AppDomain.csproj", "PmPulse.AppDomain/"]
COPY ["PmPulse.GrainInterfaces/PmPulse.GrainInterfaces.csproj", "PmPulse.GrainInterfaces/"]
COPY ["PmPulse.GrainClasses/PmPulse.GrainClasses.csproj", "PmPulse.GrainClasses/"]
RUN dotnet restore "PmPulse.FeedSiloHost/PmPulse.FeedSiloHost.csproj"

# Copy source code
COPY . .
WORKDIR "/src/PmPulse.FeedSiloHost"
RUN dotnet build "PmPulse.FeedSiloHost.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "PmPulse.FeedSiloHost.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 11111
EXPOSE 30000

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PmPulse.FeedSiloHost.dll"]

