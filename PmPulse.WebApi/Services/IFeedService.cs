using PmPulse.AppDomain.Models.Feed;
using PmPulse.AppDomain.Models.Post;

namespace PmPulse.WebApi.Services
{
    public interface IFeedService
    {
        IEnumerable<IFeed> GetFeedsByBlockName(string blockName);
        IFeed? GetFeedBySlug(string slug);

        Task InitializeFeedsAsync();

        Task<IFeedPosts> GetBlockFeedPostsAsync(string slug);
        Task<IFeedPosts> GetFeedPostsAsync(string slug);

        Task<IEnumerable<IFeedPosts>> GetWeeklyDigestAsync();
    }
}
