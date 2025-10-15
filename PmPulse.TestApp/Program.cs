using PmPulse.TestApp;

// Test the ChannelParser
try
{
    Console.WriteLine("Testing ChannelParser...");
    
    // Example usage - replace with an actual channel name
    var channel = await ChannelParserEx.ParseChannelAsync("selihovkin", limit: 5);
    
    Console.WriteLine($"Channel: {channel.Name}");
    Console.WriteLine($"URL: {channel.Url}");
    Console.WriteLine($"Messages count: {channel.Messages.Count}");
    
    foreach (var message in channel.Messages)
    {
        Console.WriteLine($"- Message: {message.Text?[..Math.Min(100, message.Text?.Length ?? 0)]}...");
        Console.WriteLine($"  Created: {message.CreatedAt}");
        Console.WriteLine($"  URL: {message.Url}");
        Console.WriteLine($"  Photo: {message.Photo}");
        Console.WriteLine();
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}
