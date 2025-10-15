using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PmPulse.AppDomain.Models.Post
{
    public interface IFeedPosts
    {
        DateTime SyncDate { get; }
        IEnumerable<IFeedPost> Posts { get; }
    }
}
