using PmPulse.AppDomain.Models.Post;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PmPulse.GrainInterfaces.Models
{
    [GenerateSerializer]
    public class FeedPost(string feedPostText, string feedPostUrl, DateTime feedPostDate, string feedPostImage = "")
            : IFeedPost
    {
        [Id(0)] public string PostText { get; init; } = feedPostText;
        [Id(1)] public string PostUrl { get; init; } = feedPostUrl;
        [Id(2)] public string PostImage { get; init; } = feedPostImage;
        [Id(3)] public DateTime PostDate { get; init; } = feedPostDate;
    }

    [GenerateSerializer]
    public class FeedPosts(DateTime lastSyncDate, IEnumerable<IFeedPost> feedPosts) : IFeedPosts
    {
        [Id(0)]
        public DateTime SyncDate { get; init; } = lastSyncDate;
        [Id(1)]
        public IEnumerable<IFeedPost> Posts { get; init; } = feedPosts;
    }

    public static class FeedPostsFactory
    {
        public static IFeedPost CreateFeedPost(string postText, string postUrl, DateTime postDate, string postImage = "")
        {
            return new FeedPost(postText, postUrl, postDate, postImage);
        }

        public static IFeedPosts CreateFeedPosts(DateTime lastSyncDate,
            IEnumerable<IFeedPost> posts, int limit = -1)
        {
            var sortedPosts = posts
                .OrderByDescending(p => p.PostDate)
                .ToList();

            if (limit > 0)
            {
                sortedPosts = sortedPosts.Take(limit).ToList();
            }

            return new FeedPosts(lastSyncDate, sortedPosts);
        }
    }
}
