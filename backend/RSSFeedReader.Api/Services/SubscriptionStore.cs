using RSSFeedReader.Api.Models;

namespace RSSFeedReader.Api.Services;

// In-memory, insertion-ordered subscription store. Lost on process restart (FR-005).
public class SubscriptionStore
{
    private readonly List<Subscription> _subscriptions = [];
    private readonly object _lock = new();

    public IReadOnlyList<Subscription> GetAll()
    {
        lock (_lock)
        {
            return _subscriptions.ToList();
        }
    }

    public Subscription Add(string url)
    {
        var subscription = new Subscription(url);
        lock (_lock)
        {
            _subscriptions.Add(subscription);
        }
        return subscription;
    }
}
