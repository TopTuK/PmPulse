namespace PmPulse.WebApi.Models.Configuration
{
    public class FeedSettings
    {
        public string Id { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public int Limit { get; set; } = 20;
        public string BlockName { get; set; } = string.Empty;
        public int BlockLimit { get; set; } = 5;
        public int DelaySeconds { get; set; } = 5;
        public int UpdateMinutes { get; set; } = 5;
    }
}
