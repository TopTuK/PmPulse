using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmPulse.AppDomain.Models.Feed
{
    public enum FeedType: byte
    {
        None = 0,
        Telegram = 1,
        Rss = 2,
    }
}
