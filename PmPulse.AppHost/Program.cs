using Microsoft.Extensions.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var startupType = Environment.GetEnvironmentVariable("STARTUP_TYPE")?.ToLower();
var IsDevelopment = !builder.Environment.IsDevelopment();

if (startupType == "docker")
{
    // Add Redis for Orleans clustering
    var redis = builder.AddRedis("redis");

    // Start Orleans Silo Host in Docker
    var feedSiloHost = builder.AddDockerfile("pmpulse-silohost", "..", "PmPulse.FeedSiloHost/Dockerfile")
        .WithHttpEndpoint(port: 11111, targetPort: 11111, name: "silo")
        .WithHttpEndpoint(port: 30000, targetPort: 30000, name: "gateway")
        .WithEnvironment("ASPNETCORE_ENVIRONMENT", IsDevelopment ? "Development" : "Docker")
        .WithEnvironment("DOTNET_ENVIRONMENT", IsDevelopment ? "Development" : "Docker")
        .WithEnvironment("ORLEANS_SILO_PORT", "11111")
        .WithEnvironment("ORLEANS_GATEWAY_PORT", "30000")
        .WithReference(redis);

    // Start frontend in Docker
    var front = builder.AddDockerfile("front", "../webapp", "Dockerfile")
        .WithHttpEndpoint(port: 5173, targetPort: 80, name: "http");

    // Start Web API in Docker
    var webApi = builder.AddDockerfile("pmpulse-webapi", "..", "PmPulse.WebApi/Dockerfile")
        .WithHttpEndpoint(port: 8080, targetPort: 8080, name: "http")
        .WithEnvironment("ASPNETCORE_ENVIRONMENT", IsDevelopment ? "Development" : "Docker")
        .WithEnvironment("DOTNET_ENVIRONMENT", IsDevelopment ? "Development" : "Docker")
        .WithEnvironment("ASPNETCORE_URLS", "http://+:8080")
        .WithReference(redis);
}
else
{
    var front = builder.AddNpmApp("front", "../webapp", "dev");
    var feedSiloHost = builder.AddProject<Projects.PmPulse_FeedSiloHost>("pmpulse-siloHost")
        .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
        .WithEnvironment("DOTNET_ENVIRONMENT", "Development");
    var webApi = builder
        .AddProject<Projects.PmPulse_WebApi>("pmpulse-webapi")
        .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
        .WithEnvironment("DOTNET_ENVIRONMENT", "Development")
        .WithReference(front)
        .WithReference(feedSiloHost)
        .WaitFor(feedSiloHost);
}

builder
    .Build()
    .Run();
