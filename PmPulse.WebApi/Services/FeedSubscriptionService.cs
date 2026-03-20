
using System.Threading;

namespace PmPulse.WebApi.Services
{
    public class FeedSubscriptionService(ILogger<FeedSubscriptionService> logger, IFeedService feedService) : BackgroundService
    {
        private const int SUBSCRIPTION_TIMER_PERIOD_MINUTES = 10;

        private readonly ILogger<FeedSubscriptionService> _logger = logger;
        private readonly IFeedService _feedService = feedService;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("FeedSubscriptionService::ExecuteAsync: START Feed Subscription Service");

            using PeriodicTimer timer = new(TimeSpan.FromMinutes(SUBSCRIPTION_TIMER_PERIOD_MINUTES));

            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                _logger.LogInformation("FeedSubscriptionService::ExecuteAsync: timer tick. START resubscribe to feed update");
                await _feedService.UpdateFeedSubscriptionsAsync();
                _logger.LogInformation("FeedSubscriptionService::ExecuteAsync: timer tick. END resubscribe to feed update");
            }

            _logger.LogInformation("FeedSubscriptionService::ExecuteAsync: STOP Feed Subscription Service");
        }
    }
}
