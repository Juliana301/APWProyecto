using System.Text.Json;
using NewsHub.Domain.Entities;
using NewsHub.Infrastructure.Repositories;

namespace NewsHub.Services.Services;

public class ParsingService
{
    private readonly HttpClient _httpClient;
    private readonly SourceItemRepository _sourceItemRepository;

    public ParsingService(
        HttpClient httpClient,
        SourceItemRepository sourceItemRepository)
    {
        _httpClient = httpClient;
        _sourceItemRepository = sourceItemRepository;
    }

    public async Task ParseSourceAsync(string url, int sourceId)
    {
        var response = await _httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
            return;

        var json = await response.Content.ReadAsStringAsync();

        using var document = JsonDocument.Parse(json);

        if (!document.RootElement.TryGetProperty("articles", out var articles))
            return;

        foreach (var article in articles.EnumerateArray())
        {
            var title = article.GetProperty("title").GetString();
            var description = article.GetProperty("description").GetString();
            var link = article.GetProperty("url").GetString();
            var image = article.GetProperty("urlToImage").GetString();

            // Convertimos la noticia a JSON
            var articleData = new
            {
                title = title ?? "",
                description = description ?? "",
                url = link ?? "",
                imageUrl = image ?? ""
            };

            var item = new SourceItem
            {
                SourceId = sourceId,
                Json = JsonSerializer.Serialize(articleData),
                CreatedAt = DateTime.UtcNow
            };

            await _sourceItemRepository.AddAsync(item);
        }
    }
}