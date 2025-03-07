using Hub.Models;

namespace Hub.Services
{
    public interface IFileService
    {
        Task UploadFile(IFormFile file, int userId);
        Task<List<FileModel>> GetFiles();
        Task<List<LinkModel>> GetLinksByFile(int fileId);
    }
}