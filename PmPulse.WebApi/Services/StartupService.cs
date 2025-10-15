
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
                _logger.LogInformation("StartupService::StartAsync: start grains initializations");
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var feedService = scope.ServiceProvider.GetRequiredService<IFeedService>();
                    await feedService.InitializeFeedsAsync();
                }
                _logger.LogInformation("StartupService::StartAsync: grains initializations complete");
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
