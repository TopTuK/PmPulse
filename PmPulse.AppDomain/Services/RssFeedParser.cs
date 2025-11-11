using CodeHollow.FeedReader;
using CodeHollow.FeedReader.Feeds;
using PmPulse.AppDomain.Models.Rss;

namespace PmPulse.AppDomain.Services
{
    public static class RssFeedParser
    {
        private const int DefaultRequestTimeout = 60;

        private static async Task<Feed> GetHtmlRssFeedAsync(string feedUrl)
        {
            using var httpClient = new HttpClient();

            httpClient.Timeout = TimeSpan.FromSeconds(DefaultRequestTimeout);

            httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
            httpClient.DefaultRequestHeaders.Add("Accept", "application/rss+xml, application/xml, text/xml, */*");
            httpClient.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.9");

            // Download the feed content with custom headers
            var feedContent = await httpClient.GetStringAsync(feedUrl);

            // Parse the feed content
            var feed = FeedReader.ReadFromString(feedContent);

            return feed;
        }

        private static async Task<Feed> GetRssFeedAsync(string feedUrl, RssFeedReaderType readerType)
        {
            var feed = (readerType) switch
            {
                RssFeedReaderType.Default => await FeedReader.ReadAsync(feedUrl),
                RssFeedReaderType.Html => await GetHtmlRssFeedAsync(feedUrl),
                _ => throw new ArgumentException("Unknown feed reader type", nameof(readerType))
            };

            return feed;
        }

        public static async Task<RssFeedSource> ParseRssFeedAsync(string feedUrl,
            int limit = 1000, RssFeedReaderType readerType = RssFeedReaderType.Default)
        {
            var feed = await GetRssFeedAsync(feedUrl, readerType);

            var rssEntries = new List<RssFeedEntry>(feed.Items.Count);
            foreach (var item in feed.Items.Take(limit))
            {
                var rssText = !string.IsNullOrEmpty(item.Description)
                    ? $"{item.Title}\\n{item.Description}"
                    : item.Title;

                string? imageUrl = null;
                if (feed.Type == FeedType.Rss_2_0)
                {
                    var rssItem = (Rss20FeedItem)item.SpecificItem;
                    if ((rssItem.Enclosure != null) && (rssItem.Enclosure.Url != null))
                    {
                        imageUrl = rssItem.Enclosure.Url;
                    }
                }
                rssEntries.Add(new RssFeedEntry(item.Link, rssText, imageUrl, item.PublishingDate ?? DateTime.Now));
            }

            return new RssFeedSource(feed.Link, feed.Title, rssEntries);
        }
    }
}
