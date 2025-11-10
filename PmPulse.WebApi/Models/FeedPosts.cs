using PmPulse.AppDomain.Models.Feed;
using PmPulse.AppDomain.Models.Post;

namespace PmPulse.WebApi.Models
{
    internal class FeedPosts : IFeedPosts
    {
        public IFeed Feed { get; init; }
        public DateTime? LastSyncDate { get; init; }
        public IEnumerable<IFeedPost>? Posts { get; init; }

        public FeedPosts(IFeed feed, ISyncedPosts? posts)
        {
            Feed = feed;
            LastSyncDate = posts?.LastSyncDate;
            Posts = posts?.Posts;
        }

        public FeedPosts(IFeed feed, DateTime syncDate, IEnumerable<IFeedPost> posts)
        {
            Feed = feed;
            LastSyncDate = syncDate;
            Posts = posts;
        }
    }
}
