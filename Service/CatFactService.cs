using CatFact.Models;
using System.Net.Http.Json;
using Microsoft.Extensions.Options;

namespace CatFact.Service;


public class CatFactService : ICatFactService
{
    private readonly HttpClient _httpClient;
    private readonly CatFactOption _options;
    private readonly ILogger<CatFactService> _logger;
    private static readonly SemaphoreSlim FileLock = new(1, 1);

    public CatFactService(
        HttpClient httpClient,
        IOptions<CatFactOption> options,
        ILogger<CatFactService> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<CatFactDto?> FetchAndSaveFactAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Pobieranie faktu o kotach {Url}", _options.ApiUrl);

        CatFactDto? fact;

        try
        {
            fact = await _httpClient.GetFromJsonAsync<CatFactDto>(_options.ApiUrl, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Otrzymano pustą odpowiedz z Api.");
            return null;
        }

        if (fact is null)
        {
            _logger.LogWarning("Otrzymano pustą odpowiedź z API.");
            return null;
        }

        String logEntry = $"[{DateTime.UtcNow:yyyy-MM-dd}] Length: {fact.Lenght} {fact.Fact}";

        await FileLock.WaitAsync(cancellationToken);
        try
        {
            await File.AppendAllLinesAsync(_options.FilePath, new[] { logEntry }, cancellationToken);
            _logger.LogInformation("Zapis faktu do pliku {FilePath}.", _options.FilePath);
        }
        finally
        {
            FileLock.Release();
        }

        return fact;
    }
}
