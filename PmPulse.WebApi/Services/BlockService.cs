using Microsoft.Extensions.Options;
using PmPulse.AppDomain.Models.Block;
using PmPulse.AppDomain.Models.Feed;
using PmPulse.WebApi.Models.Configuration;

namespace PmPulse.WebApi.Services
{
    internal class BlockService : IBlockService
    {
        private class FeedBlock : IFeedBlock
        {
            public string Name { get; init; } = string.Empty;
            public string Slug { get; init; } = string.Empty;
            public string Title { get; init; } = string.Empty;
            public string Description { get; init; } = string.Empty;
            public bool IsDefault { get; init; } = false;

            public IEnumerable<IFeed> Feeds { get; init; } = [];

            public override string ToString()
            {
                return $"FeedBlock [Name={Name}, Slug={Slug}]";
            }
        }

        private readonly ILogger<BlockService> _logger;
        private readonly List<FeedBlock> _blocks;

        public BlockService(ILogger<BlockService> logger,
            IFeedService feedService,
            IOptions<FeedBlockSettings> settings)
        {
            _logger = logger;

            _blocks = settings.Value
                .Blocks
                .Select(b =>
                {
                    var feeds = feedService.GetFeedsByBlockName(b.Name);
                    return new FeedBlock
                    {
                        Name = b.Name,
                        Slug = b.Slug,
                        Title = b.Title,
                        Description = b.Description,
                        IsDefault = b.IsDefault,
                        Feeds = feeds,
                    };
                })
                .ToList();
        }

        public IEnumerable<IFeedBlockBase> GetFeedBlocks()
        {
            _logger.LogInformation("BlockService::GetFeedBlocks: return feed blocks. " +
                "FeedBlocksCount={feedBlocksCount}", _blocks.Count);
            return _blocks;
        }

        public IFeedBlock? GetFeedBlockBySlug(string slug)
        {
            _logger.LogInformation("BlockService::GetFeedBlockBySlug: get feed block by slug. " +
                "Slug={blockSlug}", slug);
            var block = _blocks.FirstOrDefault(b => b.Slug == slug);
            
            _logger.LogInformation("BlockService::GetFeedBlockBySlug: return feed block by slug. " +
                "Slug={blockSlug} Block={feedBlock} FeedsCount={feedsCount}", slug, block, block?.Feeds.Count());
            return block;
        }
    }
}
