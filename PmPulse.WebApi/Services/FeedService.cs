using Microsoft.Extensions.Options;
using PmPulse.AppDomain.Models.Feed;
using PmPulse.AppDomain.Models.Post;
using PmPulse.GrainInterfaces;
using PmPulse.GrainInterfaces.Models;
using PmPulse.WebApi.Models;
using PmPulse.WebApi.Models.Configuration;

namespace PmPulse.WebApi.Services
{
    public class FeedService(
            ILogger<FeedService> logger, 
            IOptions<FeedBlockSettings> settings,
            IClusterClient clusterClient
        ) : IFeedService
    {
        private class Feed : IFeed
        {
            public Guid Id { get; init; }
            public FeedType FeedType { get; init; } = FeedType.None;
            public string Slug { get; init; } = string.Empty;
            public string Title { get; init; } = string.Empty;
            public string Description { get; init; } = string.Empty;
            public string Url { get; init; } = string.Empty;
            public int Limit { get; init; } = 20;
            public string BlockName { get; init; } = string.Empty;
            public int BlockLimit { get; init; } = 5;

            public int DelaySeconds { get; init; }
            public int UpdateMinutes { get; init; }
            public bool IncludeWeeklyDigest { get; init; }
        }

        private readonly ILogger<FeedService> _logger = logger;
        private readonly List<Feed> _feeds = settings.Value
                .Feeds
                .Select(f => new Feed
                {
                    Id = Guid.Parse(f.Id),
                    FeedType = (FeedType) f.FeedType,
                    Slug = f.Slug,
                    Title = f.Title,
                    Description = f.Description,
                    Url = f.Url,
                    Limit = f.Limit,
                    BlockName = f.BlockName,
                    BlockLimit = f.BlockLimit,
                    DelaySeconds = f.DelaySeconds,
                    UpdateMinutes = f.UpdateMinutes,
                    IncludeWeeklyDigest = f.IncludeWeeklyDigest,
                })
                .ToList();
        private readonly IClusterClient _clusterClient = clusterClient;

        public IEnumerable<IFeed> GetFeedsByBlockName(string blockName)
        {
            _logger.LogInformation("FeedService::GetFeedsByBlockName: start get feeds by block name. " +
                "BlockName={blockName} FeedsCount={feedsCount}", blockName, _feeds.Count);

            var feeds = _feeds
                .Where(f => f.BlockName == blockName).
                ToList();

            _logger.LogInformation("FeedService::GetFeedsByBlockName: returns feeds for block name. " +
                "BlockName={blockName}, FeedsCount={feedsCount}", blockName, feeds.Count);
            return feeds;
        }

        public async Task InitializeFeedsAsync()
        {
            _logger.LogInformation("FeedService::InitializeFeedsAsync: start initialize feed grains");

            var feedGrainsTasks = _feeds
                .Select(feed =>
                {
                    var feedGrain = _clusterClient.GetGrain<IFeedGrain>(feed.Id);
                    return feedGrain.InitializeState(
                        feed.Slug, 
                        feed.Url, 
                        feed.DelaySeconds, 
                        feed.UpdateMinutes,
                        feed.FeedType
                    );
                })
                .ToList();

            await Task.WhenAll(feedGrainsTasks);

            _logger.LogInformation("FeedService::InitializeFeedsAsync: stop initialize feed grains");
        }

        public async Task<IFeedPosts> GetBlockFeedPostsAsync(string slug)
        {
            _logger.LogInformation("FeedService::GetBlockFeedPostsAsync: get feeds for block. " +
                "Slug={slug}", slug);

            var feed = _feeds.FirstOrDefault(f => f.Slug == slug);
            if (feed == null)
            {
                _logger.LogWarning("FeedService::GetBlockFeedPostsAsync: feed is not found. " +
                    "Slug={slug}", slug);
                throw new Exception("FeedService::GetBlockFeedPostsAsync: feed is not found");
            }

            var feedGrain = _clusterClient.GetGrain<IFeedGrain>(feed.Id);
            var posts = await feedGrain.GetFeedPosts(feed.BlockLimit);
            _logger.LogInformation("FeedService::GetBlockFeedPostsAsync: return feed posts. " +
                "Slug={slug} FeedPosts={feedPosts}", slug, posts?.Posts.Count());

            return new FeedPosts(feed, posts);
        }

        public async Task<IFeedPosts> GetFeedPostsAsync(string slug)
        {
            _logger.LogInformation("FeedService::GetFeedPostsAsync: get feeds posts. " +
                "Slug={slug}", slug);

            var feed = _feeds.FirstOrDefault(f => f.Slug == slug);
            if (feed == null)
            {
                _logger.LogWarning("FeedService::GetFeedPostsAsync: feed is not found. " +
                    "Slug={slug}", slug);
                throw new Exception("FeedService::GetFeedPostsAsync: feed is not found");
            }

            var feedGrain = _clusterClient.GetGrain<IFeedGrain>(feed.Id);
            var posts = await feedGrain.GetFeedPosts(feed.Limit);

            _logger.LogInformation("FeedService::GetFeedPostsAsync: return feed posts. " +
                "Slug={slug} FeedPosts={feedPosts}", slug, posts?.Posts.Count());
            return new FeedPosts(feed, posts);
        }

        public IFeed? GetFeedBySlug(string slug)
        {
            _logger.LogInformation("FeedService::GetFeedBySlug: start get feed by slug. " +
                "Slug={slug}", slug);

            var feed = _feeds.FirstOrDefault(f => f.Slug.Equals(slug));
            _logger.LogInformation("FeedService::GetFeedBySlug: return feed by slug. " +
                "Slug={slug} Feed={feed}", slug, feed);
            return feed;
        }

        public async Task<IEnumerable<IFeedPosts>> GetWeeklyDigestAsync()
        {
            const int WEEK_DIGEST_DAYS = -7;

            _logger.LogInformation("FeedService::GetWeeklyDigestAsync: start get weekly digest");

            var digestFeeds = _feeds
                .Where(f => f.IncludeWeeklyDigest)
                .Select(f => f)
                .ToList();
            _logger.LogInformation("FeedService::GetWeeklyDigestAsync: got digest feeds. " +
                "Count={digestFeedsCount}", digestFeeds.Count);

            var today = DateTime.Today.Date;
            var startDate = DateTime.Today.AddDays(WEEK_DIGEST_DAYS);

            var weeklyPosts = await digestFeeds
                .Select(f => new
                {
                    Feed = f,
                    Grain = _clusterClient.GetGrain<IFeedGrain>(f.Id)
                })
                .ToAsyncEnumerable()
                .SelectAwait(async fg => new
                {
                    fg.Feed,
                    Posts = await fg.Grain.GetPostsByDate(startDate)
                })
                .Select(fp => new FeedPosts(fp.Feed, today, fp.Posts))
                .ToListAsync();

            _logger.LogInformation("FeedService::GetWeeklyDigestAsync: return weekly digesr. " +
                "WeeklyFeedPostsCount={weeklyFeedPostsCount}", weeklyPosts.Count);
            return weeklyPosts;
        }
    }
}
