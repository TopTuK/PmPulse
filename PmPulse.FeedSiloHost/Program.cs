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
        .UseOrleans(silo =>
        {
            silo.UseLocalhostClustering()
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