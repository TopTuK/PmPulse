# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["PmPulse.WebApi/PmPulse.WebApi.csproj", "PmPulse.WebApi/"]
COPY ["PmPulse.ServiceDefaults/PmPulse.ServiceDefaults.csproj", "PmPulse.ServiceDefaults/"]
COPY ["PmPulse.AppDomain/PmPulse.AppDomain.csproj", "PmPulse.AppDomain/"]
COPY ["PmPulse.GrainInterfaces/PmPulse.GrainInterfaces.csproj", "PmPulse.GrainInterfaces/"]
COPY ["PmPulse.GrainClasses/PmPulse.GrainClasses.csproj", "PmPulse.GrainClasses/"]
RUN dotnet restore "PmPulse.WebApi/PmPulse.WebApi.csproj"

# Copy source code
COPY . .
WORKDIR "/src/PmPulse.WebApi"
RUN dotnet build "PmPulse.WebApi.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "PmPulse.WebApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PmPulse.WebApi.dll"]

