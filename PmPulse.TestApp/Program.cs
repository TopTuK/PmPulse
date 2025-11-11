using CodeHollow.FeedReader;

var httpClient = new HttpClient();
httpClient.DefaultRequestHeaders.Add("User-Agent", 
    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
httpClient.DefaultRequestHeaders.Add("Accept", "application/rss+xml, application/xml, text/xml, */*");
httpClient.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.9");

try
{
    Console.WriteLine("Testing RssParser...");

    // Try different Reddit RSS URL formats
    const string feedUrl = "https://www.reddit.com/r/news/.rss";

    // Download the feed content with custom headers
    var feedContent = await httpClient.GetStringAsync(feedUrl);
    
    // Parse the feed content
    var feed = FeedReader.ReadFromString(feedContent);

    Console.WriteLine($"Feed Title: {feed.Title}");
    Console.WriteLine($"Feed Items: {feed.Items.Count()}");
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
    Console.WriteLine($"Stack Trace: {ex.StackTrace}");
}
