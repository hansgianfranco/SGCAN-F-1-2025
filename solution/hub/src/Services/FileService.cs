using Hub.Data;
using Hub.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Text;

namespace Hub.Services
{
    public class FileService : IFileService
    {
        private readonly AppDbContext _context;
        private readonly IQueueService _queueService;
        private readonly ILogger<FileService> _logger;

        public FileService(AppDbContext context, IQueueService queueService, ILogger<FileService> logger)
        {
            _context = context;
            _queueService = queueService;
            _logger = logger;
        }

        public async Task UploadFile(IFormFile file, int userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    _logger.LogError("Usuario con ID {UserId} no encontrado.", userId);
                    throw new Exception("Usuario no encontrado");
                }

                var email = user.Username;
                var fileModel = new FileModel
                {
                    FileName = file.FileName,
                    UploadDate = DateTime.UtcNow,
                    Status = "Processing",
                    UserId = userId
                };

                _context.Files.Add(fileModel);
                await _context.SaveChangesAsync();

                var links = await ExtractLinksFromFile(file);

                var linkModels = links.Select(link => new LinkModel
                {
                    Url = link,
                    FileModelId = fileModel.Id,
                    Content = ""
                }).ToList();

                _context.Links.AddRange(linkModels);
                await _context.SaveChangesAsync();

                await _queueService.ProcessLinks(links, email);

                fileModel.Status = "Completed";
                await _context.SaveChangesAsync();

                _logger.LogInformation("Archivo {FileName} subido y encolado con éxito.", file.FileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar el archivo {FileName}", file.FileName);
                throw;
            }
        }

        public async Task<List<FileModel>> GetFiles()
        {
            return await _context.Files
            .Include(f => f.Links) 
            .ToListAsync();
        }

        public async Task<List<LinkModel>> GetLinksByFile(int fileId)
        {
            return await _context.Links
                .Where(l => l.FileModelId == fileId)
                .ToListAsync();
        }

         private async Task<List<string>> ExtractLinksFromFile(IFormFile file)
        {
            var links = new List<string>();

            using (var stream = file.OpenReadStream())
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                string? line;

                while ((line = await reader.ReadLineAsync()) != null)
                {
                    if (!string.IsNullOrWhiteSpace(line) && Uri.IsWellFormedUriString(line, UriKind.Absolute))
                    {
                        links.Add(line);
                    }
                }
            }

            return links;
        }
    }
}