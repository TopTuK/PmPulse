using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmPulse.AppDomain.Models.Post
{
    public interface ISyncedPosts
    {
        DateTime LastSyncDate { get; }
        IEnumerable<IFeedPost> Posts { get; }
    }
}
