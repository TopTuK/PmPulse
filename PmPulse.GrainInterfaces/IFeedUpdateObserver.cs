using PmPulse.AppDomain.Models.Post;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmPulse.GrainInterfaces
{
    public interface IFeedUpdateObserver : IGrainObserver
    {
        Task OnFeedUpdate(Guid feedId, string slug, IEnumerable<IFeedPost> posts);
    }
}
