using Microsoft.Extensions.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var startupType = Environment.GetEnvironmentVariable("STARTUP_TYPE")?.ToLower();
var IsNotDevelopment = !builder.Environment.IsDevelopment();

if (startupType == "docker")
{
    // Add PostgreSQL for Orleans clustering using AddContainer for full control
    // Note: We don't set POSTGRES_USER to allow default "postgres" user
    // POSTGRES_PASSWORD sets the password for the default "postgres" user (default: "orleans")
    // The orleans user and database will be created via init script
    var postgresPassword = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD") ?? "orleans";
    var postgres = builder.AddContainer("postgres", "postgres:16-alpine")
        .WithEnvironment("POSTGRES_PASSWORD", postgresPassword)
        .WithBindMount("../docker-entrypoint-initdb.d", "/docker-entrypoint-initdb.d")
        .WithEndpoint(5432, 5432, "postgres");

    // Build the webapp frontend using NPM (Vite) for static assets
    var webappBuild = builder.AddNpmApp("webapp-build", "../webapp", "build");

    // Start Orleans Silo Host in Docker
    var feedSiloHost = builder.AddDockerfile("pmpulse-silohost", "..", "PmPulse.FeedSiloHost/Dockerfile")
        .WithHttpEndpoint(port: 11111, targetPort: 11111, name: "silo")
        .WithHttpEndpoint(port: 30000, targetPort: 30000, name: "gateway")
        .WithEnvironment("ASPNETCORE_ENVIRONMENT", IsNotDevelopment ? "Development" : "Docker")
        .WithEnvironment("DOTNET_ENVIRONMENT", IsNotDevelopment ? "Development" : "Docker")
        .WithEnvironment("ORLEANS_SILO_PORT", "11111")
        .WithEnvironment("ORLEANS_GATEWAY_PORT", "30000")
        .WithEnvironment("POSTGRES_CONNECTION_STRING", $"Host=postgres;Database=orleans;Username=orleans;Password={postgresPassword}");

    // Start frontend in Docker
    // var front = builder.AddDockerfile("front", "../webapp", "Dockerfile")
        // .WithHttpEndpoint(port: 5173, targetPort: 80, name: "http");

    // Start Web API in Docker
    var webApi = builder.AddDockerfile("pmpulse-webapi", "..", "PmPulse.WebApi/Dockerfile")
        .WaitFor(webappBuild)
        .WithHttpEndpoint(port: 8080, targetPort: 8080, name: "http")
        .WithEnvironment("ASPNETCORE_ENVIRONMENT", IsNotDevelopment ? "Development" : "Docker")
        .WithEnvironment("DOTNET_ENVIRONMENT", IsNotDevelopment ? "Development" : "Docker")
        .WithEnvironment("ASPNETCORE_URLS", "http://+:8080")
        .WithEnvironment("POSTGRES_CONNECTION_STRING", $"Host=postgres;Database=orleans;Username=orleans;Password={postgresPassword}");
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
