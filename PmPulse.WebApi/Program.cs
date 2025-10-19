using PmPulse.WebApi.Models.Configuration;
using PmPulse.WebApi.Services;
using Serilog;
using System.ComponentModel;

static void ConfigureServices(IServiceCollection services)
{
    // HostedService
    services.AddHostedService<StartupService>();

    // Singletons
    services.AddSingleton<IBlockService, BlockService>();
    services.AddSingleton<IFeedService,  FeedService>();
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

    // Configure options
    ConfigureOptions(builder.Services, configuration);

    // Configure application services
    ConfigureServices(builder.Services);

    // Add controllers to the container
    builder.Services
        .AddControllers()
        .AddNewtonsoftJson();

    // Add Orleans client
    builder.UseOrleansClient(client =>
    {
        var environment = builder.Environment;
        
        if (environment.IsDevelopment() && Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") != "true")
        {
            // Use localhost clustering for local development
            client.UseLocalhostClustering();
        }
        else
        {
            // Use Docker/Kubernetes networking
            var siloHost = Environment.GetEnvironmentVariable("ORLEANS_SILO_HOST") ?? "pmpulse-silohost";
            var gatewayPort = int.Parse(Environment.GetEnvironmentVariable("ORLEANS_GATEWAY_PORT") ?? "30000");
            
            client.UseStaticClustering(new System.Net.IPEndPoint(
                System.Net.Dns.GetHostAddresses(siloHost)[0], 
                gatewayPort));
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

    app.UseStaticFiles();
    app.UseRouting();

    // https://habr.com/ru/articles/468401/
    /*
    app.UseMiddleware<UserAuthMiddleware>();
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseMiddleware<UserIdMiddleware>();
    */

#pragma warning disable ASP0014 // Suggest using top level route registrations
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllerRoute(
            name: "default",
            pattern: "api/{controller}/{action=Index}/{id?}"
        );
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