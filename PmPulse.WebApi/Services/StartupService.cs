
namespace PmPulse.WebApi.Services
{
    public class StartupService(ILogger<StartupService> logger,
        IHostApplicationLifetime hostApplicationLifetime,
        IServiceScopeFactory serviceScopeFactory) : IHostedService
    {
        private readonly ILogger<StartupService> _logger = logger;
        private readonly IHostApplicationLifetime _appLifetime = hostApplicationLifetime;
        private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("StartupService::StartAsync: start initializing");

            _appLifetime.ApplicationStarted.Register(async () =>
            {
                _logger.LogInformation("StartupService::StartAsync: waiting for Orleans client to connect...");
                
                // Wait for Orleans client to be ready with retry logic
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var clusterClient = scope.ServiceProvider.GetService<Orleans.IClusterClient>();
                    if (clusterClient != null)
                    {
                        var maxRetries = 30; // 30 retries = 5 minutes max
                        var retryDelay = TimeSpan.FromSeconds(10);
                        var retryCount = 0;
                        
                        while (retryCount < maxRetries && !cancellationToken.IsCancellationRequested)
                        {
                            try
                            {
                                // Test connectivity by attempting to get a grain reference
                                // This will throw if the client is not connected
                                var testGrain = clusterClient.GetGrain<PmPulse.GrainInterfaces.IFeedGrain>(Guid.Empty);
                                
                                // If we got here, the client is connected
                                _logger.LogInformation("StartupService::StartAsync: Orleans client connected successfully");
                                break;
                            }
                            catch (Exception ex) when (!(ex is InvalidOperationException))
                            {
                                retryCount++;
                                _logger.LogInformation("StartupService::StartAsync: Orleans client not ready yet (attempt {RetryCount}/{MaxRetries}), waiting {Delay}s... Error: {Error}", 
                                    retryCount, maxRetries, retryDelay.TotalSeconds, ex.Message);
                                
                                if (retryCount >= maxRetries)
                                {
                                    _logger.LogError(ex, "StartupService::StartAsync: Orleans client failed to connect after {MaxRetries} attempts. This may indicate that no silo gateways are available.", maxRetries);
                                    throw new InvalidOperationException($"Orleans client failed to connect after {maxRetries} attempts. Ensure silos are running and gateways are registered in Redis.", ex);
                                }
                                
                                await Task.Delay(retryDelay, cancellationToken);
                            }
                        }
                    }
                    else
                    {
                        _logger.LogWarning("StartupService::StartAsync: Orleans IClusterClient not found in service provider");
                    }
                    
                    _logger.LogInformation("StartupService::StartAsync: start grains initializations");
                    var feedService = scope.ServiceProvider.GetRequiredService<IFeedService>();
                    await feedService.InitializeFeedsAsync();
                    _logger.LogInformation("StartupService::StartAsync: grains initializations complete");
                }
            });

            _logger.LogInformation("StartupService::StartAsync: stop initializing");

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
