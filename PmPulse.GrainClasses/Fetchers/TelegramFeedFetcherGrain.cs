using Microsoft.Extensions.Logging;
using PmPulse.AppDomain.Models;
using PmPulse.AppDomain.Models.Post;
using PmPulse.AppDomain.Services;
using PmPulse.GrainInterfaces;
using PmPulse.GrainInterfaces.Models;

namespace PmPulse.GrainClasses.Fetchers
{
    public class TelegramFeedFetcherGrain(
        ILogger<TelegramFeedFetcherGrain> logger,
        [PersistentState(OrleansConstants.FETCHER_STATE_NAME, OrleansConstants.FEED_STATE_STORE_NAME)]
            IPersistentState<FeedFetcherState> feedFetcheState
        ) : Grain, ITelegramFeedFetcherGrain, IRemindable
    {
        private readonly ILogger<TelegramFeedFetcherGrain> _logger = logger;
        private readonly IPersistentState<FeedFetcherState> _feedFetcherState = feedFetcheState;

        private IGrainReminder _reminder = null!;

        private async Task<IEnumerable<IFeedPost>> FetchTelegramChannel(string channelName)
        {
            _logger.LogInformation("TelegramFeedFetcherGrain::FetchTelegramChannel: start fetch telegram posts. " +
                "ChannelName={channelName}", channelName);

            var channel = await TelegramChannelParser.ParseChannelAsync(channelName);
            _logger.LogInformation("TelegramFeedFetcherGrain::FetchTelegramChannel: complete parse Telegram channel. " +
                "ChannelName={channelName}, ChannelUrl={channelUrl}, MessagesCount={msgCount}",
                channel.Name, channel.Url, channel.Messages.Count);

            var posts = channel.Messages
                .Select(m => FeedPostsFactory.CreateFeedPost(
                    m.Text ?? "Empty text",
                    m.Url ?? string.Empty,
                    m.CreatedAt,
                    m.Photo ?? string.Empty
                ))
                .ToList();

            _logger.LogInformation("TelegramFeedFetcherGrain::FetchTelegramChannel: return feed posts. " +
                "ChannelName={channelName} PostsCount={postsCount}", channelName, posts.Count);
            return posts;
        }

        public async Task ReceiveReminder(string reminderName, TickStatus status)
        {
            var grainId = this.GetPrimaryKey();
            var slug = _feedFetcherState.State.Slug;

            _logger.LogInformation("TelegramFeedFetcherGrain::ReceiveReminder: start execute reminder. " +
                "GrainId={grainId} ReminderName={reminiderName} Slug={slug}", grainId, reminderName, slug);

            if (reminderName == slug)
            {
                var channelName = _feedFetcherState.State.Url;
                var posts = await FetchTelegramChannel(channelName);

                _logger.LogInformation("TelegramFeedFetcherGrain::ReceiveReminder: set posts to feed grain. " +
                    "GrainId={grainId}, PostsCount={postsCount}", grainId, posts.Count());
                var feedGrain = GrainFactory.GetGrain<IFeedGrain>(grainId);
                await feedGrain.SetPosts(posts);
            }
            
            _logger.LogInformation("TelegramFeedFetcherGrain::ReceiveReminder: stop execute reminder. " +
                "GrainId={grainId} ReminderName={reminiderName} Slug={slug}", grainId, reminderName, slug);
        }

        public async Task StartFetch(string slug, string url, int delaySeconds, int updateMinutes)
        {
            var grainId = this.GetPrimaryKey();
            _logger.LogInformation("TelegramFeedFetcherGrain::StartFetch: starting fetching news. " +
                "GrainId={grainId} Slug={slug} Url={url}", grainId, slug, url);

            _reminder = await this.RegisterOrUpdateReminder(
                slug, 
                TimeSpan.FromSeconds(delaySeconds), 
                TimeSpan.FromMinutes(updateMinutes)
            );
            _logger.LogInformation("TelegramFeedFetcherGrain::StartFetch: registered reminder. " +
                "GrainId={grainId} Slug={slug} Url={url}", grainId, slug, url);

            _feedFetcherState.State.Url = url;
            _feedFetcherState.State.Slug = slug;
            _feedFetcherState.State.LastUpdateDate = null;
            _feedFetcherState.State.DelayIntervalSeconds = delaySeconds;
            _feedFetcherState.State.UpdateIntervalMinutes = updateMinutes;
            await _feedFetcherState.WriteStateAsync();
            _logger.LogInformation("TelegramFeedFetcherGrain::StartFetch: writed state. " +
                "GrainId={grainId} Slug={slug} Url={url}",
                grainId, _feedFetcherState.State.Slug, _feedFetcherState.State.Url);

            _logger.LogInformation("TelegramFeedFetcherGrain::StartFetch: completed starting fetching news. " +
                "GrainId={grainId} Slug={slug} Url={url}", grainId, slug, url);
        }

        public async Task StopFetch()
        {
            if (_reminder != null)
            {
                await this.UnregisterReminder(_reminder);
                _reminder = null!;
            }
        }
    }
}
