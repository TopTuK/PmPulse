using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PmPulse.AppDomain.Models.Feed
{
    public interface IFeed
    {
        [JsonIgnore] Guid Id { get; }
        string Slug { get; }
        string Title { get; }
        string Description { get; }
        [JsonIgnore] string Url { get; }
        [JsonIgnore] int Limit { get; }
        [JsonIgnore] string BlockName { get; }
        [JsonIgnore] int BlockLimit { get; }
        [JsonIgnore] int DelaySeconds { get; }
        [JsonIgnore] int UpdateMinutes { get; }
    }
}
