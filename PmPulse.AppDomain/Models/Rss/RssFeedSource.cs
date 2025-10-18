using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmPulse.AppDomain.Models.Rss
{
    public record RssFeedSource(string Url, string Title, List<RssFeedEntry> Entries);
}
