using StackExchange.Redis;

namespace Hub.Services
{
    public class QueueService : IQueueService
    {
        private readonly IConnectionMultiplexer _redis;

        public QueueService(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }

        public async Task Enqueue(string message)
        {
            var db = _redis.GetDatabase();
            await db.ListRightPushAsync("links_queue", message);
        }
    }
}