using Hub.Services;
using Hub.Models;
using Microsoft.AspNetCore.Mvc;

namespace Hub.Controllers
{
    [ApiController]
    [Route("api")]
    public class FileController : ControllerBase
    {
        private readonly IFileService _fileService;

        public FileController(IFileService fileService)
        {
            _fileService = fileService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            await _fileService.UploadFile(file);
            return Ok(new { message = "File uploaded successfully." });
        }

        [HttpGet("files")]
        public async Task<IActionResult> GetFiles()
        {
            var files = await _fileService.GetFiles();
            return Ok(files);
        }

        [HttpGet("files/{fileId}/links")]
        public async Task<IActionResult> GetLinksByFile(int fileId)
        {
            var links = await _fileService.GetLinksByFile(fileId);
            return Ok(links);
        }
    }
}