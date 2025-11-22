using PmPulse.AppDomain.Models.Post;
using PmPulse.GrainInterfaces;

namespace PmPulse.WebApi.Services
{
    public class FeedUpdateObserver(ILogger<FeedUpdateObserver> logger) : IFeedUpdateObserver
    {
        private readonly ILogger<FeedUpdateObserver> _logger = logger;

        public async Task OnFeedUpdate(Guid feedId, string slug, IEnumerable<IFeedPost> posts)
        {
            _logger.LogInformation("FeedUpdateObserver::OnFeedUpdate: got feed update. " +
                "FeedId={feedId}, Slug={slug}", feedId, slug);

            _logger.LogInformation("FeedUpdateObserver::OnFeedUpdate: end update feed posts. " +
                "FeedId={feedId}, Slug={slug}", feedId, slug);
        }
    }
}
