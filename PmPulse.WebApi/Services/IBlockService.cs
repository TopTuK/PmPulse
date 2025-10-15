using PmPulse.AppDomain.Models.Block;

namespace PmPulse.WebApi.Services
{
    public interface IBlockService
    {
        IEnumerable<IFeedBlockBase> GetFeedBlocks();
        IFeedBlock? GetFeedBlockBySlug(string slug);
    }
}
