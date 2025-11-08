using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PmPulse.WebApi.Services;

namespace PmPulse.WebApi.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class FeedPostController : ControllerBase
    {
        private readonly ILogger<FeedPostController> _logger;
        private readonly IBlockService _blockService;
        private readonly IFeedService _feedService;

        public FeedPostController(ILogger<FeedPostController> logger, 
            IFeedService feedService,
            IBlockService blockService)
        {
            _logger = logger;
            _feedService = feedService;
            _blockService = blockService;
        }

        public async Task<IActionResult> GetBlockFeedPosts(string slug)
        {
            _logger.LogInformation("FeedPostController::GetBlockFeedPosts: start get feeds for block. " +
                "Slug={slug}", slug);

            try
            {
                var feedPosts = await _feedService.GetBlockFeedPostsAsync(slug);

                if (feedPosts == null)
                {
                    _logger.LogWarning("FeedPostController::GetBlockFeedPosts: feed is not initialized. " +
                        "Slug={slug}", slug);
                    return NotFound();
                }

                _logger.LogInformation("FeedPostController::GetBlockFeedPosts: return feed posts. " +
                    "Slug={slug} LastSyncDate={lastSyncDate} PostsCount={postsCount}",
                    slug, feedPosts.SyncDate, feedPosts.Posts.Count());
                return new JsonResult(feedPosts);
            }
            catch (Exception ex)
            {
                _logger.LogError("FeedPostController::GetBlockFeedPosts: exception raised. " +
                    "Message={exMsg}", ex.Message);
                return BadRequest("Feed is not found");
            }
        }

        public async Task<IActionResult> GetFeedPosts(string slug)
        {
            _logger.LogInformation("FeedPostController::GetFeedPosts: start get feed posts. " +
                "Slug={slug}", slug);

            try
            {
                var feed = _feedService.GetFeedBySlug(slug);
                if (feed == null)
                {
                    _logger.LogWarning("FeedPostController::GetFeedPosts: can\'t find feed by slug. " +
                        "Slug={slug}", slug);
                    return NotFound();
                }

                var feedPosts = await _feedService.GetFeedPostsAsync(slug);

                if (feedPosts == null)
                {
                    _logger.LogWarning("FeedPostController::GetFeedPosts: feed is not initialized. " +
                        "Slug={slug}", slug);
                    return NotFound();
                }

                _logger.LogInformation("FeedPostController::GetBlockFeedPosts: return feed posts. " +
                    "Slug={slug} LastSyncDate={lastSyncDate} PostsCount={postsCount}",
                    slug, feedPosts.SyncDate, feedPosts.Posts.Count());
                return new JsonResult(new
                {
                    feed,
                    feedPosts
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("FeedPostController::GetFeedPosts: exception raised. " +
                    "Message={exMsg}", ex.Message);
                return BadRequest("Feed is not found");
            }
        }

        public async Task<IActionResult> GetWeeklyDigest()
        {
            _logger.LogInformation("FeedPostController::GetWeeklyDigest: start get weekly digest. ");

            try
            {
                //var weeklyDigest = await _feedService.GetWeeklyDigestAsync();
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                _logger.LogError("FeedPostController::GetWeeklyDigest: exception raised. " +
                    "Message={exMsg}", ex.Message);
                return BadRequest("Weekly digest is not found");
            }
        }
    }
}
