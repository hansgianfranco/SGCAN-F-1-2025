using System.Collections.Generic;
using System.Threading.Tasks;

public interface IQueueService
{
    Task ProcessLinks(List<string> links, string email);
}