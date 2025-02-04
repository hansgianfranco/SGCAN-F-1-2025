namespace Hub.Services
{
    public interface IQueueService
    {
        Task Enqueue(string message);
    }
}