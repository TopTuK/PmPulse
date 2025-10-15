using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PmPulse.WebApi.Services;

namespace PmPulse.WebApi.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class FeedBlockController : ControllerBase
    {
        private readonly ILogger<FeedBlockController> _logger;
        private readonly IBlockService _blockService;
        private readonly IFeedService _feedService;

        public FeedBlockController(
            ILogger<FeedBlockController> logger, 
            IBlockService blockService,
            IFeedService feedService)
        {
            _logger = logger;
            _blockService = blockService;
            _feedService = feedService;
        }

        public IActionResult GetFeedBlock(string slug = "main")
        {
            _logger.LogInformation("FeedBlockController::GetFeedBlock: start get feed block. " +
                "Slug=\"{slug}\"", slug);
            
            var blockFeed = _blockService.GetFeedBlockBySlug(slug);
            if (blockFeed == null)
            {
                _logger.LogWarning("FeedBlockController::GetFeedBlock: can\'t find feed Block. " +
                    "Slug=\"{slug}\"", slug);
                return NotFound(slug);
            }

            _logger.LogInformation("FeedBlockController::GetFeedBlock: return feed block. " +
                "Slug={slug} Title={title}, FeedsCount={feedsCount}", 
                blockFeed.Slug, blockFeed.Title, blockFeed.Feeds.Count());
            return new JsonResult(blockFeed);
        }

        public IActionResult GetAllFeedBlocks()
        {
            _logger.LogInformation("FeedBlockController::GetAllFeedBlocks: start get feed blocks");

            var feedBlocks = _blockService
                .GetFeedBlocks()
                .Select(b => new
                {
                    b.Title,
                    b.Slug
                })
                .ToList();

            _logger.LogInformation("FeedBlockController::GetAllFeedBlocks: return feed blocks. " +
                "Count={feedBlocksCount}", feedBlocks.Count);
            return new JsonResult(feedBlocks);
        }
    }
}
