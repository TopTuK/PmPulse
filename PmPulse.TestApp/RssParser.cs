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
        public static async ValueTask ParseRssFeedAsync(string feedUrl, HttpClient? httpClient = null)
        {
            // If no HttpClient provided, create one with default headers for Reddit RSS
            if (httpClient == null)
            {
                httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("User-Agent",
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
                httpClient.DefaultRequestHeaders.Add("Accept", "application/rss+xml, application/xml, text/xml, */*");
                httpClient.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.9");
            }

            // Download the feed content with custom headers
            var feedContent = await httpClient.GetStringAsync(feedUrl);
            
            // Parse the feed content
            var feed = FeedReader.ReadFromString(feedContent);

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
