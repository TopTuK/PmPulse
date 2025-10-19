using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PmPulse.AppDomain.Models;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

try
{
    var builder = Host.CreateDefaultBuilder(args);

    builder
        .UseOrleans((context, silo) =>
        {
            var environment = context.HostingEnvironment;
            
            if (environment.IsDevelopment() && Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") != "true")
            {
                // Use localhost clustering for local development
                silo.UseLocalhostClustering();
            }
            else
            {
                // Use Docker/Kubernetes networking
                var siloPort = int.Parse(Environment.GetEnvironmentVariable("ORLEANS_SILO_PORT") ?? "11111");
                var gatewayPort = int.Parse(Environment.GetEnvironmentVariable("ORLEANS_GATEWAY_PORT") ?? "30000");
                var advertisedIP = Environment.GetEnvironmentVariable("ORLEANS_ADVERTISED_IP") ?? "pmpulse-silohost";
                
                silo.UseKubernetesHosting()
                    .ConfigureEndpoints(advertisedIP, siloPort, gatewayPort, listenOnAnyHostAddress: true);
            }
            
            silo.Configure<Orleans.Configuration.ClusterOptions>(options =>
                {
                    options.ClusterId = "dev";
                    options.ServiceId = "PmPulse";
                })
                .UseInMemoryReminderService()
                .AddMemoryGrainStorage(OrleansConstants.FEED_STATE_STORE_NAME)
                .AddMemoryGrainStorage(OrleansConstants.POST_STATE_STORE_NAME)
                .AddMemoryGrainStorage(OrleansConstants.FETCHER_STATE_STORE_NAME)
                .ConfigureLogging(logging => logging.AddConsole());
        })
        .UseConsoleLifetime();

    using IHost host = builder.Build();

    await host.RunAsync();
}
catch(Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}