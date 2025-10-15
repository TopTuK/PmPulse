using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmPulse.GrainClasses.Fetchers
{
    [GenerateSerializer]
    public record FeedFetcherState
    {
        [Id(0)] public DateTime? LastUpdateDate { get; set; } = null;
        [Id(1)] public string Slug { get; set; } = string.Empty;
        [Id(2)] public string Url { get; set; } = string.Empty;
        [Id(3)] public int DelayIntervalSeconds { get; set; } = 5;
        [Id(4)] public int UpdateIntervalMinutes { get; set; } = 5;
    }
}
