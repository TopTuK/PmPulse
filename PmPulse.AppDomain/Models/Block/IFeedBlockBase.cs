using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmPulse.AppDomain.Models.Block
{
    public interface IFeedBlockBase
    {
        string Name { get; }
        string Slug { get; }
        string Title { get; }
        string Description { get; }
        bool IsDefault { get; }
    }
}
