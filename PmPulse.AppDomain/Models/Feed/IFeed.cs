using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;
using PmPulse.AppDomain.Models.Rss;

namespace PmPulse.AppDomain.Models.Feed
{
    public interface IFeed
    {
        [JsonIgnore] Guid Id { get; }
        [JsonIgnore] FeedType FeedType { get; }
        string Slug { get; }
        string Title { get; }
        string Description { get; }
        [JsonIgnore] string Url { get; }
        [JsonIgnore] int Limit { get; }
        [JsonIgnore] string BlockName { get; }
        [JsonIgnore] int BlockLimit { get; }
        [JsonIgnore] int DelaySeconds { get; }
        [JsonIgnore] int UpdateMinutes { get; }
        [JsonIgnore] bool IncludeWeeklyDigest { get; }
        [JsonIgnore] FeedReaderType ReaderType { get; }
    }
}
