using Microsoft.AspNetCore.SignalR;
using PmPulse.AppDomain.Models.Post;
using PmPulse.GrainInterfaces;
using PmPulse.WebApi.Hubs;

namespace PmPulse.WebApi.Services
{
    public class FeedUpdateObserver(
        ILogger<FeedUpdateObserver> logger,
        IHubContext<FeedUpdateHub> hubContext) : IFeedUpdateObserver
    {
        private readonly ILogger<FeedUpdateObserver> _logger = logger;
        private readonly IHubContext<FeedUpdateHub> _hubContext = hubContext;

        public async Task OnFeedUpdate(Guid feedId, string slug, IEnumerable<IFeedPost> posts)
        {
            _logger.LogInformation("FeedUpdateObserver::OnFeedUpdate: got feed update. " +
                "FeedId={feedId}, Slug={slug}", feedId, slug);

            try
            {
                // Notify all connected clients about the feed update
                await _hubContext.Clients.All.SendAsync("FeedUpdated", new
                {
                    FeedId = feedId,
                    Slug = slug,
                    Posts = posts.Select(p => new
                    {
                        PostText = p.PostText,
                        PostUrl = p.PostUrl,
                        PostImage = p.PostImage,
                        PostDate = p.PostDate
                    })
                });

                _logger.LogInformation("FeedUpdateObserver::OnFeedUpdate: notified clients via SignalR. " +
                    "FeedId={feedId}, Slug={slug}, PostCount={postCount}", 
                    feedId, slug, posts.Count());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "FeedUpdateObserver::OnFeedUpdate: error notifying clients via SignalR. " +
                    "FeedId={feedId}, Slug={slug}", feedId, slug);
            }

            _logger.LogInformation("FeedUpdateObserver::OnFeedUpdate: end update feed posts. " +
                "FeedId={feedId}, Slug={slug}", feedId, slug);
        }
    }
}
