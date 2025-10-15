using PmPulse.AppDomain.Models.Post;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmPulse.GrainClasses.State
{
    [GenerateSerializer]
    public record FeedPostsState
    {
        [Id(0)] public DateTime? SyncDate { get; set; } = null;
        [Id(2)] public IEnumerable<IFeedPost> Posts { get; set; } = [];
    }
}
