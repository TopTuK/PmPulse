namespace PmPulse.WebApi.Models.Configuration
{
    public class BlockSettings
    {
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Title {  get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsDefault { get; set; } = false;
    }
}
