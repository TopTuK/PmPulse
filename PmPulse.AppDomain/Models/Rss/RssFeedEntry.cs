using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmPulse.AppDomain.Models.Rss
{
    public record RssFeedEntry(string? Url, string Text, string? ImageUrl, DateTime CreatedAt);
}
