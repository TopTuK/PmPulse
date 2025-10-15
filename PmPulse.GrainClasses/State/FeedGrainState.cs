using PmPulse.AppDomain.Models.Feed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmPulse.GrainClasses.State
{
    [GenerateSerializer]
    public record FeedGrainState
    {
        [Id(0)] public FeedState CurrentState { get; set; } = FeedState.None;
        [Id(1)] public string Slug { get; set; } = string.Empty;
        [Id(2)] public string Url { get; set; } = string.Empty;
    }
}
