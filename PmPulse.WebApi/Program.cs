using Microsoft.AspNetCore.StaticFiles;
using PmPulse.GrainInterfaces;
using PmPulse.WebApi.Hubs;
using PmPulse.WebApi.Models;
using PmPulse.WebApi.Models.Configuration;
using PmPulse.WebApi.Services;
using Serilog;
using System.ComponentModel;
using System.Security.Cryptography.X509Certificates;

static void ConfigureServices(IServiceCollection services)
{
    // HostedService
    services.AddHostedService<StartupService>();
    services.AddHostedService<FeedSubscriptionService>();

    // Singletons
    services.AddSingleton<IBlockService, BlockService>();
    services.AddSingleton<IFeedService,  FeedService>();
    services.AddSingleton<IFeedUpdateObserver, FeedUpdateObserver>();
}

static void ConfigureOptions(IServiceCollection services, IConfiguration configuration)
{
    services.Configure<FeedBlockSettings>(configuration.GetSection("FeedBlockSettings"));
}

// Configure logger
// https://github.com/serilog/serilog-aspnetcore
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);
    var configuration = builder.Configuration;

    // Configure Kestrel for HTTPS with PEM certificates (non-development environments)
    if (!builder.Environment.IsDevelopment())
    {
        var certPath = Environment.GetEnvironmentVariable("ASPNETCORE_Kestrel__Certificates__Default__CertificatePath");
        var keyPath = Environment.GetEnvironmentVariable("ASPNETCORE_Kestrel__Certificates__Default__CertificateKeyPath");
        
        if (!string.IsNullOrEmpty(certPath) && !string.IsNullOrEmpty(keyPath) && File.Exists(certPath) && File.Exists(keyPath))
        {
            builder.WebHost.ConfigureKestrel(options =>
            {
                options.ConfigureHttpsDefaults(httpsOptions =>
                {
                    httpsOptions.ServerCertificate = X509Certificate2.CreateFromPemFile(certPath, keyPath);
                });
            });
        }
    }

    // Configure options
    ConfigureOptions(builder.Services, configuration);

    // Configure application services
    ConfigureServices(builder.Services);

    // Add SignalR
    builder.Services.AddSignalR();

    // Add controllers to the container
    builder.Services
        .AddControllers()
        .AddNewtonsoftJson(options =>
        {
        });

    // Add Orleans client
    builder.UseOrleansClient(client =>
    {
        var environment = builder.Environment;
        
        if (environment.IsDevelopment())
        {
            // Use localhost clustering for local development
            client.UseLocalhostClustering();
        }
        else
        {
            // Use Redis clustering for Docker networking with scale support
            var redisConnectionString = Environment.GetEnvironmentVariable("REDIS_CONNECTION_STRING") 
                ?? Environment.GetEnvironmentVariable("ConnectionStrings__redis") 
                ?? "localhost:6379";
            
            client.UseRedisClustering(options =>
            {
                options.ConfigurationOptions = StackExchange.Redis.ConfigurationOptions.Parse(redisConnectionString);
            });
        }
        
        // Configure connection retry for better reliability
        client.Configure<Orleans.Configuration.ClusterOptions>(options =>
        {
            options.ClusterId = "dev";
            options.ServiceId = "PmPulse";
        });
        
        // Add retry logic for gateway connections
        client.Configure<Orleans.Configuration.GatewayOptions>(options =>
        {
            options.GatewayListRefreshPeriod = TimeSpan.FromSeconds(10);
        });
    });

    /* BUILD */
    var app = builder.Build();

    // Configure the HTTP request pipeline
    if (!app.Environment.IsDevelopment())
    {
        // The default HSTS value is 30 days. You may want to change this for production scenarios
        // see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
        app.UseHttpsRedirection();
    }

    // Configure static files with appropriate cache control
    var staticFileOptions = new StaticFileOptions();
    staticFileOptions.OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers.Append("Cache-Control", "no-cache, no-store, must-revalidate");
        ctx.Context.Response.Headers.Append("Pragma", "no-cache");
        ctx.Context.Response.Headers.Append("Expires", "0");
    };
    
    app.UseStaticFiles(staticFileOptions);
    app.UseRouting();

#pragma warning disable ASP0014 // Suggest using top level route registrations
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllerRoute(
            name: "default",
            pattern: "api/{controller}/{action=Index}/{id?}"
        );
        endpoints.MapHub<FeedUpdateHub>("/hubs/feedupdate");
    });
#pragma warning restore ASP0014 // Suggest using top level route registrations

    if (app.Environment.IsDevelopment())
    {
        app.UseSpa(spa =>
        {
            spa.UseProxyToSpaDevelopmentServer("http://localhost:5173");
        });
    }
    else
    {
        app.MapFallbackToFile("index.html");
    }

    /* RUN APP */
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}