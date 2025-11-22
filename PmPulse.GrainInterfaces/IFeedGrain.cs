using PmPulse.AppDomain.Models.Feed;
using PmPulse.AppDomain.Models.Post;
using PmPulse.AppDomain.Models.Rss;
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
        Task InitializeState(string slug, string url, 
            int delaySeconds, int updateMinutes, 
            FeedType feedType, FeedReaderType readerType);

        [Alias("SetPosts")]
        Task SetPosts(IEnumerable<IFeedPost> posts);

        [Alias("GetFeedPosts")]
        Task<ISyncedPosts?> GetFeedPosts(int limit = -1);

        [Alias("GetPostsByDate")]
        Task<IEnumerable<IFeedPost>> GetPostsByDate(DateTime startDate);


        [Alias("Subscribe")]
        Task Subscribe(IFeedUpdateObserver observer);

        [Alias("UnSubscribe")]
        Task UnSubscribe(IFeedUpdateObserver observer);
    }
}
