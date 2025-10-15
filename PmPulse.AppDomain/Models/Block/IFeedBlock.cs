using PmPulse.AppDomain.Models.Feed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmPulse.AppDomain.Models.Block
{
    public interface IFeedBlock : IFeedBlockBase
    {
        IEnumerable<IFeed> Feeds { get; }
    }
}
