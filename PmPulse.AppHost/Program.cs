var builder = DistributedApplication.CreateBuilder(args);

// Start Orleans Silo Host in Docker
var feedSiloHost = builder.AddDockerfile("pmpulse-silohost", "../PmPulse.FeedSiloHost", "Dockerfile")
    .WithHttpEndpoint(port: 11111, targetPort: 11111, name: "silo")
    .WithHttpEndpoint(port: 30000, targetPort: 30000, name: "gateway")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
    .WithEnvironment("DOTNET_RUNNING_IN_CONTAINER", "true")
    .WithEnvironment("ORLEANS_SILO_PORT", "11111")
    .WithEnvironment("ORLEANS_GATEWAY_PORT", "30000")
    .WithEnvironment("ORLEANS_ADVERTISED_IP", "pmpulse-silohost");

// Start frontend in Docker
var front = builder.AddDockerfile("front", "../webapp", "Dockerfile")
    .WithHttpEndpoint(port: 80, targetPort: 80, name: "http");

// Start Web API in Docker
var webApi = builder.AddDockerfile("pmpulse-webapi", "../PmPulse.WebApi", "Dockerfile")
    .WithHttpEndpoint(port: 8080, targetPort: 8080, name: "http")
    .WithHttpEndpoint(port: 8081, targetPort: 8081, name: "https")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
    .WithEnvironment("DOTNET_RUNNING_IN_CONTAINER", "true")
    .WithEnvironment("ASPNETCORE_URLS", "http://+:8080;https://+:8081")
    .WithEnvironment("ORLEANS_SILO_HOST", "pmpulse-silohost")
    .WithEnvironment("ORLEANS_GATEWAY_PORT", "30000")
    .WithReference(front)
    .WithReference(feedSiloHost);

builder
    .Build()
    .Run();
