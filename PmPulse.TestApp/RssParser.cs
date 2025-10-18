using CodeHollow.FeedReader;
using CodeHollow.FeedReader.Feeds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmPulse.TestApp
{
    public static class RssParser
    {
        public static async ValueTask ParseRssFeedAsync(string feedUrl)
        {
            var feed = await FeedReader.ReadAsync(feedUrl);

            Console.WriteLine("Feed Title: " + feed.Title);
            Console.WriteLine("Feed Description: " + feed.Description);
            Console.WriteLine("Feed Image: " + feed.ImageUrl);

            foreach (var item in feed.Items)
            {
                Console.WriteLine("Title: " + item.Title + " Link: " + item.Link);
                if (feed.Type == FeedType.Rss_2_0)
                {
                    var rssItem = (Rss20FeedItem)item.SpecificItem;
                    if ((rssItem.Enclosure != null) && (rssItem.Enclosure.Url != null))
                    {
                        Console.WriteLine("Enclosure URL: " + rssItem.Enclosure.Url);
                        //Console.WriteLine("Enclosure Type: " + rssItem.Enclosure.Type);
                    }
                }
            }
        }
    }
}
