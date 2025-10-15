using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmPulse.GrainInterfaces
{
    public interface IFeedFetcherGrain : IGrainWithGuidKey
    {
        Task StartFetch(string slug, string url, int delaySeconds, int updateMinutes);
        Task StopFetch();
    }
}
