using PmPulse.AppDomain.Models.Feed;
using PmPulse.AppDomain.Models.Post;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmPulse.GrainInterfaces
{
    /// <summary>
    /// Feed grain contains feed information.
    /// </summary>
    [Alias("PmPulse.GrainInterfaces.IFeedGrain")]
    public interface IFeedGrain : IGrainWithGuidKey
    {
        [Alias("InitializeState")]
        Task InitializeState(string slug, string url, int delaySeconds, int updateMinutes, FeedType feedType);

        [Alias("SetPosts")]
        Task SetPosts(IEnumerable<IFeedPost> posts);

        [Alias("GetFeedPosts")]
        Task<IFeedPosts?> GetFeedPosts(int limit = -1);
    }
}
