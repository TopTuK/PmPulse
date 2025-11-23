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
    
    // Configure Sentry (production only)
    builder.ConfigureLogging((context, logging) =>
    {
        var hostingEnvironment = context.HostingEnvironment;
        
        if (hostingEnvironment.IsProduction())
        {
            var sentryDsn = Environment.GetEnvironmentVariable("SENTRY_DSN") 
                ?? Environment.GetEnvironmentVariable("Sentry__Dsn");
            
            if (!string.IsNullOrEmpty(sentryDsn))
            {
                logging.AddSentry(options =>
                {
                    options.Dsn = sentryDsn;
                    options.TracesSampleRate = 1.0; // Capture 100% of transactions for performance monitoring
                    options.Environment = hostingEnvironment.EnvironmentName;
                });
            }
        }
    });

    builder
        .UseOrleans((context, silo) =>
        {
            var environment = context.HostingEnvironment;
            
            if (environment.IsDevelopment())
            {
                // Use localhost clustering for local development
                silo.UseLocalhostClustering();
            }
            else
            {
                // Use Docker container networking with Redis clustering for scale support
                var siloPort = int.Parse(Environment.GetEnvironmentVariable("ORLEANS_SILO_PORT") ?? "11111");
                var gatewayPort = int.Parse(Environment.GetEnvironmentVariable("ORLEANS_GATEWAY_PORT") ?? "30000");
                var advertisedIP = Environment.GetEnvironmentVariable("ORLEANS_ADVERTISED_IP") ?? Environment.GetEnvironmentVariable("HOSTNAME");
                var redisConnectionString = Environment.GetEnvironmentVariable("REDIS_CONNECTION_STRING") 
                    ?? Environment.GetEnvironmentVariable("ConnectionStrings__redis") 
                    ?? "localhost:6379";
                
                // Configure endpoints for Docker networking
                silo.ConfigureEndpoints(advertisedIP, siloPort, gatewayPort, listenOnAnyHostAddress: true);
                
                // Use Redis for clustering to support horizontal scaling
                silo.UseRedisClustering(options =>
                {
                    options.ConfigurationOptions = StackExchange.Redis.ConfigurationOptions.Parse(redisConnectionString);
                });
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