using Microsoft.AspNetCore.Connections.Features;
using Microsoft.Extensions.Logging;
using Orleans.Providers;
using PmPulse.AppDomain.Models;
using PmPulse.AppDomain.Models.Post;
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
        private readonly ILogger<FeedGrain> _logger = logger;

        private readonly IPersistentState<FeedGrainState> _feedState = feedState;
        private readonly IPersistentState<FeedPostsState> _postState = postState;

        public async Task InitializeState(string slug, string url, int delaySeconds, int updateMinutes)
        {
            var grainId = this.GetPrimaryKey();
            _logger.LogInformation("FeedGrain::InitializeState: initialize state. " +
                "GrainId={grainId} Slug={slug} Url={url}", grainId, slug, url);

            var fetcherGrain = GrainFactory.GetGrain<ITelegramFeedFetcherGrain>(grainId);
            await fetcherGrain.StartFetch(slug, url, delaySeconds, updateMinutes);
            _logger.LogInformation("FeedGrain::InitializeState: started fetch news. " +
                "GrainId={grainId} Url={url} DelaySeconds={delaySeconds}, UpdateMinutes={updateMinutes}",
                grainId, url, delaySeconds, updateMinutes);

            _feedState.State.Url = url;
            _feedState.State.Slug = slug;
            _feedState.State.CurrentState = AppDomain.Models.Feed.FeedState.Ready;
            await _feedState.WriteStateAsync();

            _logger.LogInformation("FeedGrain::InitializeState: complete initialize state. " +
            "GrainId={grainId} Slug={slug} Url={url}", grainId, slug, url);
        }

        public async Task SetPosts(IEnumerable<IFeedPost> posts)
        {
            var grainId = this.GetPrimaryKey();
            _logger.LogInformation("FeedGrain::SetPosts: start write posts to state. " +
                "GrainId={grainId} PostsCount={postsCount}", grainId, posts.Count());

            _postState.State.SyncDate = DateTime.UtcNow;
            _postState.State.Posts = posts;
            await _postState.WriteStateAsync();

            _logger.LogInformation("FeedGrain::SetPosts: complete write posts to state. " +
                "GrainId={grainId} PostsCount={postsCount}", grainId, _postState.State.Posts.Count());
        }

        public async Task<IFeedPosts?> GetFeedPosts(int limit = -1)
        {
            var grainId = this.GetPrimaryKey();
            _logger.LogInformation("FeedGrain::GetFeedPosts: start get feed posts. " +
                "GrainId={grainId} Limit={limit}", grainId, limit);

            if (_feedState.State.CurrentState == AppDomain.Models.Feed.FeedState.None)
            {
                _logger.LogWarning("FeedGrain::GetFeedPosts: feed grain is not initialized. " +
                    "GrainId={grainId}", grainId);
                return null;
            }

            var feedPosts = FeedPostsFactory.CreateFeedPosts(
                (DateTime)_postState.State.SyncDate!,
                _postState.State.Posts,
                limit);
            _logger.LogInformation("FeedGrain::GetFeedPosts: return feed posts. " +
                "GrainId={grainId} PostsSyncDate={syncDate}, PostsCount={postsCount}",
                grainId, feedPosts.SyncDate, feedPosts.Posts.Count());

            return await Task.FromResult(feedPosts);
        }
    }
}
