using Hub.Data;
using Hub.Models;
using Microsoft.EntityFrameworkCore;

namespace Hub.Services
{
    public class FileService : IFileService
    {
        private readonly AppDbContext _context;

        public FileService(AppDbContext context)
        {
            _context = context;
        }

        public async Task UploadFile(IFormFile file)
        {
            var fileModel = new FileModel
            {
                FileName = file.FileName,
                UploadDate = DateTime.UtcNow,
                Status = "Pending"
            };

            _context.Files.Add(fileModel);
            await _context.SaveChangesAsync();

            // Procesar los links del archivo (aquí puedes enviar a Redis)
        }

        public async Task<List<FileModel>> GetFiles()
        {
            return await _context.Files.ToListAsync();
        }

        public async Task<List<LinkModel>> GetLinksByFile(int fileId)
        {
            return await _context.Links
                .Where(l => l.FileModelId == fileId)
                .ToListAsync();
        }
    }
}