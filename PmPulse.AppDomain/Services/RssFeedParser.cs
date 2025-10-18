using CodeHollow.FeedReader;
using CodeHollow.FeedReader.Feeds;
using PmPulse.AppDomain.Models.Rss;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmPulse.AppDomain.Services
{
    public static class RssFeedParser
    {
        public static async Task<RssFeedSource> ParseRssFeedAsync(string feedUrl, int limit = 1000)
        {
            var feed = await FeedReader.ReadAsync(feedUrl);

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
