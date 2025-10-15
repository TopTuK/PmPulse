var builder = DistributedApplication.CreateBuilder(args);

// Start frontend
var front = builder.AddNpmApp("front", "../webapp", "dev");

var feedSiloHost = builder.AddProject<Projects.PmPulse_FeedSiloHost>("pmpulse-silohost");

var webApi = builder
    .AddProject<Projects.PmPulse_WebApi>("pmpulse-webapi")
    .WithReference(front)
    .WithReference(feedSiloHost);

builder
    .Build()
    .Run();
