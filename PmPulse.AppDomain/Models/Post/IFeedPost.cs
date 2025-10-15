using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PmPulse.AppDomain.Models.Post
{
    public interface IFeedPost
    {
        string PostText { get; }
        string PostUrl { get; }
        string PostImage { get; }
        DateTime PostDate { get; }
    }
}
