using System.Text.Json;
using System.Xml.Linq;
using NewsHub.Domain.Entities;
using NewsHub.Infrastructure.Repositories;

namespace NewsHub.Services.Services;

public class ParsingService
{
    private readonly HttpClient _httpClient;
    private readonly SourceItemRepository _sourceItemRepository;
    private readonly SourceRepository _sourceRepository;

    public ParsingService(
        HttpClient httpClient,
        SourceItemRepository sourceItemRepository,
        SourceRepository sourceRepository)
    {
        _httpClient = httpClient;
        _sourceItemRepository = sourceItemRepository;
        _sourceRepository = sourceRepository;
    }

    public async Task FetchAllSourcesAsync()
    {
        var sources = await _sourceRepository.GetAllAsync();

        foreach (var source in sources)
        {
            await ParseSourceAsync(source.Url, source.Id);
        }
    }

    public async Task ParseSourceAsync(string url, int sourceId)
    {
        var response = await _httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
            return;

        var content = await response.Content.ReadAsStringAsync();

        // Detectar si es XML o JSON
        if (content.TrimStart().StartsWith("<"))
        {
            await ParseRssAsync(content, sourceId);
        }
        else
        {
            await ParseJsonAsync(content, sourceId);
        }
    }

    private async Task ParseJsonAsync(string json, int sourceId)
    {
        using var document = JsonDocument.Parse(json);

        if (!document.RootElement.TryGetProperty("articles", out var articles))
            return;

        foreach (var article in articles.EnumerateArray())
        {
            var title = article.GetProperty("title").GetString();
            var description = article.GetProperty("description").GetString();
            var link = article.GetProperty("url").GetString();
            var image = article.GetProperty("urlToImage").GetString();

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

    private async Task ParseRssAsync(string xmlContent, int sourceId)
    {
        var xml = XDocument.Parse(xmlContent);

        var items = xml.Descendants("item");

        foreach (var item in items)
        {
            var title = item.Element("title")?.Value;
            var description = item.Element("description")?.Value;
            var link = item.Element("link")?.Value;

            var image = item
                .Descendants()
                .FirstOrDefault(x => x.Name.LocalName == "content")
                ?.Attribute("url")?.Value;

            var articleData = new
            {
                title = title ?? "",
                description = description ?? "",
                url = link ?? "",
                imageUrl = image ?? ""
            };

            var newsItem = new SourceItem
            {
                SourceId = sourceId,
                Json = JsonSerializer.Serialize(articleData),
                CreatedAt = DateTime.UtcNow
            };

            await _sourceItemRepository.AddAsync(newsItem);
        }
    }
}