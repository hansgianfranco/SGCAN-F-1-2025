using Hub.Services;
using Hub.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;

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

            var token = Request.Headers["Authorization"].ToString()?.Replace("Bearer ", "");

            if (string.IsNullOrEmpty(token))
                return Unauthorized("Token is missing or invalid.");

            var userId = GetUserIdFromToken(token);
            if (userId == null)
                return Unauthorized("User ID not found in token.");

            await _fileService.UploadFile(file, userId.Value);
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

        private int? GetUserIdFromToken(string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

                if (jwtToken != null)
                {
                    var userIdClaim = jwtToken?.Claims.FirstOrDefault(c => c.Type == "userId");
                    if (userIdClaim != null)
                    {
                        return int.Parse(userIdClaim.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al procesar el token JWT: " + ex.Message);
            }
            return null;
        }
    }
}