namespace PmPulse.WebApi.Models.Configuration
{
    public class FeedBlockSettings
    {
        public List<FeedSettings> Feeds { get; set; } = [];
        public List<BlockSettings> Blocks { get; set; } = [];
    }
}
