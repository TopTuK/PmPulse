using Microsoft.AspNetCore.Connections.Features;
using Microsoft.Extensions.Logging;
using Orleans.Providers;
using Orleans.Utilities;
using PmPulse.AppDomain.Models;
using PmPulse.AppDomain.Models.Feed;
using PmPulse.AppDomain.Models.Post;
using PmPulse.AppDomain.Models.Rss;
using PmPulse.GrainClasses.State;
using PmPulse.GrainInterfaces;
using PmPulse.GrainInterfaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmPulse.GrainClasses
{
    public class FeedGrain(
        ILogger<FeedGrain> logger,
        [PersistentState(OrleansConstants.FEED_STATE_NAME, OrleansConstants.FEED_STATE_STORE_NAME)]
                IPersistentState<FeedGrainState> feedState,
        [PersistentState(OrleansConstants.POST_STATE_NAME, OrleansConstants.POST_STATE_STORE_NAME)]
                IPersistentState<FeedPostsState> postState) : Grain, IFeedGrain
    {
        private const int OBSERVER_EXPIRATION_MINUTES = 15;

        private readonly ILogger<FeedGrain> _logger = logger;

        private readonly IPersistentState<FeedGrainState> _feedState = feedState;
        private readonly IPersistentState<FeedPostsState> _postState = postState;

        private readonly ObserverManager<IFeedUpdateObserver> _subsManager = new(
            TimeSpan.FromMinutes(OBSERVER_EXPIRATION_MINUTES),
            logger
        );

        public override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            return base.OnActivateAsync(cancellationToken);
        }

        public async Task InitializeState(string slug, string url, 
            int delaySeconds, int updateMinutes,
            FeedType feedType, FeedReaderType readerType)
        {
            var grainId = this.GetPrimaryKey();
            _logger.LogInformation("FeedGrain::InitializeState: initialize state. " +
                "GrainId={grainId} Slug={slug} Url={url}", grainId, slug, url);

            _logger.LogInformation("FeedGrain::InitializeState: fetcher grain. " +
                "GrainId={grainId} FetcherGrainType={feedType}", grainId, feedType.ToString());
            IFeedFetcherGrain fetcherGrain = (feedType) switch
            {
                FeedType.Telegram => GrainFactory.GetGrain<ITelegramFeedFetcherGrain>(grainId),
                FeedType.Rss => GrainFactory.GetGrain<IRssFeedFetcherGrain>(grainId),
                _ => throw new ArgumentException("Unknown feed type", nameof(feedType))
            };

            await fetcherGrain.StartFetch(slug, url, delaySeconds, updateMinutes, readerType);
            _logger.LogInformation("FeedGrain::InitializeState: started fetch news. " +
                "GrainId={grainId} Url={url} DelaySeconds={delaySeconds}, UpdateMinutes={updateMinutes}",
                grainId, url, delaySeconds, updateMinutes);

            _feedState.State.Url = url;
            _feedState.State.Slug = slug;
            _feedState.State.CurrentState = FeedState.Ready;
            _feedState.State.FeedType = feedType;
            await _feedState.WriteStateAsync();

            _logger.LogInformation("FeedGrain::InitializeState: complete initialize state. " +
            "GrainId={grainId} Slug={slug} Url={url}", grainId, slug, url);
        }

        public async Task SetPosts(IEnumerable<IFeedPost> posts)
        {
            var grainId = this.GetPrimaryKey();
            var now = DateTime.UtcNow;
            var postsCount = posts.Count();

            _logger.LogInformation("FeedGrain::SetPosts: save posts to state. " +
                "GrainId={grainId} PostsCount={postsCount} SyncDate={now}", grainId, postsCount, now);

            _postState.State.SyncDate = now;
            _postState.State.Posts = posts;
            await _postState.WriteStateAsync();
            _logger.LogInformation("FeedGrain::SetPosts: saved posts to state. " +
                "GrainId={grainId} PostsCount={postsCount} SyncDate={now}", grainId, postsCount, now);

            var slug = _feedState.State.Slug;
            await _subsManager.Notify(s => s.OnFeedUpdate(grainId, slug, posts));

            _logger.LogInformation("FeedGrain::SetPosts: complete write posts to state. " +
                "GrainId={grainId} PostsCount={postsCount}", grainId, _postState.State.Posts.Count());
        }

        public async Task<ISyncedPosts?> GetFeedPosts(int limit = -1)
        {
            var grainId = this.GetPrimaryKey();
            _logger.LogInformation("FeedGrain::GetFeedPosts: start get feed posts. " +
                "GrainId={grainId} Limit={limit}", grainId, limit);

            if (_feedState.State.CurrentState == FeedState.None)
            {
                _logger.LogWarning("FeedGrain::GetFeedPosts: feed grain is not initialized. " +
                    "GrainId={grainId}", grainId);
                return null;
            }
            else if (_postState.State.SyncDate is null)
            {
                _logger.LogInformation("FeedGrain::GetFeedPosts: LastSyncDate sync date is null");
                return null;
            }

            var feedPosts = FeedPostsFactory.CreateFeedPosts(
                (DateTime)_postState.State.SyncDate!,
                _postState.State.Posts,
                limit);
            _logger.LogInformation("FeedGrain::GetFeedPosts: return feed posts. " +
                "GrainId={grainId} PostsSyncDate={syncDate}, PostsCount={postsCount}",
                grainId, feedPosts.LastSyncDate, feedPosts.Posts.Count());

            return await Task.FromResult(feedPosts);
        }

        public Task<IEnumerable<IFeedPost>> GetPostsByDate(DateTime startDate)
        {
            var grainId = this.GetPrimaryKey();
            _logger.LogInformation("FeedGrain::GetPostsByDate: start get weekly posts. " +
                "GrainId={grainId} Start date={startDate}", grainId, startDate.ToString("d"));

            if (_feedState.State.CurrentState == FeedState.None)
            {
                _logger.LogWarning("FeedGrain::GetPostsByDate: feed grain is not initialized. " +
                    "GrainId={grainId}", grainId);
                return Task.FromResult(Enumerable.Empty<IFeedPost>());
            }
            else if (_postState.State.SyncDate is null)
            {
                _logger.LogInformation("FeedGrain::GetPostsByDate: LastSyncDate sync date is null");
                return Task.FromResult(Enumerable.Empty<IFeedPost>()); ;
            }

            var feedPosts = _postState.State.Posts
                .Where(p => p.PostDate >= startDate)
                .Select(p => FeedPostsFactory.CreateFeedPost(
                    p.PostText,
                    p.PostUrl,
                    p.PostDate,
                    p.PostImage
                ))
                .ToList();

            _logger.LogInformation("FeedGrain::GetPostsByDate: return weekly feed posts." +
                "PostsCount={postsCount}", feedPosts.Count);
            return Task.FromResult<IEnumerable<IFeedPost>>(feedPosts);
        }

        public Task Subscribe(IFeedUpdateObserver observer)
        {
            _subsManager.Subscribe(observer, observer);
            return Task.CompletedTask;
        }

        public Task UnSubscribe(IFeedUpdateObserver observer)
        {
            _subsManager.Unsubscribe(observer);
            return Task.CompletedTask;
        }
    }
}
