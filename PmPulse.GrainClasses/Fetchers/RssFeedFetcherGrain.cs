using Microsoft.Extensions.Logging;
using PmPulse.AppDomain.Models;
using PmPulse.AppDomain.Models.Post;
using PmPulse.AppDomain.Models.Rss;
using PmPulse.AppDomain.Services;
using PmPulse.GrainInterfaces;
using PmPulse.GrainInterfaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace PmPulse.GrainClasses.Fetchers
{
    public class RssFeedFetcherGrain(
        ILogger<RssFeedFetcherGrain> logger,
        [PersistentState(OrleansConstants.FETCHER_STATE_NAME, OrleansConstants.FEED_STATE_STORE_NAME)]
            IPersistentState<FeedFetcherState> feedFetcheState
        ) : Grain, IRssFeedFetcherGrain, IRemindable
    {
        private readonly ILogger<RssFeedFetcherGrain> _logger = logger;
        private readonly IPersistentState<FeedFetcherState> _feedFetcherState = feedFetcheState;

        private IGrainReminder _reminder = null!;

        public async Task StartFetch(string slug, string url,
            int delaySeconds, int updateMinutes, 
            FeedReaderType readerType)
        {
            var grainId = this.GetPrimaryKey();
            _logger.LogInformation("RssFeedFetcherGrain::StartFetch: starting fetching news. " +
                "GrainId={grainId} Slug={slug} Url={url}", grainId, slug, url);

            _reminder = await this.RegisterOrUpdateReminder(
                slug,
                TimeSpan.FromSeconds(delaySeconds),
                TimeSpan.FromMinutes(updateMinutes)
            );
            _logger.LogInformation("RssFeedFetcherGrain::StartFetch: registered reminder. " +
                "GrainId={grainId} Slug={slug} Url={url}", grainId, slug, url);

            _feedFetcherState.State.Url = url;
            _feedFetcherState.State.Slug = slug;
            _feedFetcherState.State.LastUpdateDate = null;
            _feedFetcherState.State.DelayIntervalSeconds = delaySeconds;
            _feedFetcherState.State.UpdateIntervalMinutes = updateMinutes;
            _feedFetcherState.State.ReaderType = readerType;

            await _feedFetcherState.WriteStateAsync();
            _logger.LogInformation("RssFeedFetcherGrain::StartFetch: writed state. " +
                "GrainId={grainId} Slug={slug} Url={url}",
                grainId, _feedFetcherState.State.Slug, _feedFetcherState.State.Url);

            _logger.LogInformation("RssFeedFetcherGrain::StartFetch: completed starting fetching news. " +
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

        private async Task<IEnumerable<IFeedPost>> FetchRssFeedAsync(string feedUrl,
            FeedReaderType readerType = FeedReaderType.Default)
        {
            _logger.LogInformation("RssFeedFetcherGrain::FetchRssFeedAsync: start fetch RSS feed. " +
                "RssUrl={rssUrl}", feedUrl);

            try
            {
                var rssFeed = await RssFeedParser.ParseRssFeedAsync(feedUrl, 200, readerType);
                _logger.LogInformation("RssFeedFetcherGrain::FetchRssFeedAsync: complete parse RSS feed. " +
                    "RssName={rssName}, RssUrl={rssUrl}, MessagesCount={msgCount}",
                rssFeed.Title, rssFeed.Url, rssFeed.Entries.Count);

                var posts = rssFeed.Entries
                .Select(m => FeedPostsFactory.CreateFeedPost(
                    m.Text ?? "Empty text",
                    m.Url ?? string.Empty,
                    m.CreatedAt,
                    m.ImageUrl ?? string.Empty
                ))
                .ToList();

                _logger.LogInformation("RssFeedFetcherGrain::FetchRssFeedAsync: return rss feed posts. " +
                "RssName={rssName} PostsCount={postsCount}", rssFeed.Title, posts.Count);
                return posts;
            }
            catch (Exception ex)
            {
                _logger.LogError("RssFeedFetcherGrain::FetchRssFeedAsync: exception raised. Msg: {exMsg}",
                    ex.Message);
                throw;
            }
        }

        public async Task ReceiveReminder(string reminderName, TickStatus status)
        {
            var grainId = this.GetPrimaryKey();
            var slug = _feedFetcherState.State.Slug;

            _logger.LogInformation("RssFeedFetcherGrain::ReceiveReminder: start execute reminder. " +
                "GrainId={grainId} ReminderName={reminiderName} Slug={slug}", grainId, reminderName, slug);

            if (reminderName == slug)
            {
                var feedUrl = _feedFetcherState.State.Url;
                var readerType = _feedFetcherState.State.ReaderType;
                try
                {
                    var posts = await FetchRssFeedAsync(feedUrl, readerType);

                    if (posts.Any())
                    {
                        _logger.LogInformation("RssFeedFetcherGrain::ReceiveReminder: start set posts to feed grain. " +
                        "GrainId={grainId}, PostsCount={postsCount}", grainId, posts.Count());

                        var feedGrain = GrainFactory.GetGrain<IFeedGrain>(grainId);
                        await feedGrain.SetPosts(posts);

                        _logger.LogInformation("RssFeedFetcherGrain::ReceiveReminder: end set posts to feed grain. " +
                        "GrainId={grainId}, PostsCount={postsCount}", grainId, posts.Count());
                    }
                }
                catch
                {
                    _logger.LogInformation("RssFeedFetcherGrain::ReceiveReminder: exception raised. It was handled.");
                }
            }

            _logger.LogInformation("RssFeedFetcherGrain::ReceiveReminder: stop execute reminder. " +
                "GrainId={grainId} ReminderName={reminiderName} Slug={slug}", grainId, reminderName, slug);
        }
    }
}
