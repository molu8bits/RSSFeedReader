namespace RSSFeedReader.Api.Models;

// Represents a single feed subscription tracked by the MVP (URL only, no validation).
public record Subscription(string Url);
