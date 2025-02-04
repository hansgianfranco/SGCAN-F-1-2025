
using Hub.Data;
using Hub.Models;
using Hub.DTOs;
using StackExchange.Redis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Hub.Services;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Json;
using Newtonsoft.Json;

public class QueueService : IQueueService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly ILogger<QueueService> _logger;
    private readonly HttpClient _httpClient;
    private readonly AppDbContext _context;

    public QueueService(IConnectionMultiplexer redis, ILogger<QueueService> logger, HttpClient httpClient, AppDbContext context)
    {
        _redis = redis;
        _logger = logger;
        _httpClient = httpClient;
        _context = context;
    }

    public async Task ProcessLinks(List<string> links, string email)
    {
        try
        {
            var linkRequest = new { links = links, email = email };
            var content = new StringContent(JsonConvert.SerializeObject(linkRequest), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("http://localhost:9020/process", content);
            
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Notificación de scraping enviada correctamente a: {Email}", email);

                var processedLinks = await response.Content.ReadFromJsonAsync<ProcessedLinksResponse>();

                if (processedLinks?.ProcessedLinks != null)
                {
                    foreach (var link in processedLinks.ProcessedLinks)
                    {
                        var existingLink = await _context.Links.FirstOrDefaultAsync(l => l.Id == int.Parse(link.Id));

                        if (existingLink != null)
                        {
                            existingLink.Content = link.Content;
                            _context.Links.Update(existingLink);
                        }
                        else
                        {
                            _logger.LogWarning("Link con ID {LinkId} no encontrado para actualizar.", link.Id);
                        }
                    }
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("Links actualizados correctamente en la base de datos.");

                await SendNotificationAsync(email);
            }
            else
            {
                _logger.LogError("Error al enviar notificación de scraping a: {Email}. Código de estado: {StatusCode}", email, response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al procesar los links.");
            throw;
        }
    }

    private async Task SendNotificationAsync(string email)
    {
        try
        {
            var content = new StringContent(JsonConvert.SerializeObject(new { email = email }), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("http://localhost:9030/notify", content);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Notificación enviada correctamente a: {Email}", email);
            }
            else
            {
                _logger.LogError("Error al enviar notificación a: {Email}. Código de estado: {StatusCode}", email, response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al enviar la notificación.");
        }
    }
    
}

public class ProcessedLinksResponse
{
    public string Status { get; set; }
    public List<ProcessedLink> ProcessedLinks { get; set; }
}

public class ProcessedLink
{
    public string Id { get; set; }
    public string Content { get; set; }
}