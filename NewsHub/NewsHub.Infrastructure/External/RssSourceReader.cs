using NewsHub.Application.Common.Interfaces;
using NewsHub.Application.Common.Models;
using NewsHub.Application.Interfaces;
using NewsHub.Domain.Entities;
using NewsHub.Domain.Enums;
using System.Xml.Linq;

namespace NewsHub.Infrastructure.External
{
    public class RssSourceReader : ISourceReader
    {
        private readonly HttpClient _httpClient;
        private readonly ICacheService _cache;

        public RssSourceReader(HttpClient httpClient, ICacheService cache)
        {
            _httpClient = httpClient;
            _cache = cache;
        }

        public SourceType Type => SourceType.Rss;

        public async Task<List<SourceItem>> ReadAsync(SourceEnt source)
        {
            var cacheKey =
                $"rss_cache_{source.Url}";

            var cacheMinutes = 5;

            return await _cache.GetOrCreateAsync(
                cacheKey,
                async () =>
                {
                    var items =
                        new List<SourceItem>();

                    var xmlString =
                        await _httpClient
                            .GetStringAsync(source.Url);

                    var xml =
                        XDocument.Parse(xmlString);

                    var rssItems =
                        xml.Descendants("item")
                           .Take(20);

                    foreach (var rssItem in rssItems)
                    {
                        var title =
                        rssItem.Element("title")?.Value
                        ?? "Sin título";

                        var description =
                            rssItem.Element("description")?.Value
                            ?? "Sin descripción";

                        var link =
                            rssItem.Element("link")?.Value
                            ?? "#";

                        // MÚLTIPLES CATEGORÍAS
                        var categories =
                            rssItem.Elements("category")
                                .Select(c => c.Value)
                                .Where(c => !string.IsNullOrWhiteSpace(c))
                                .ToArray();

                        if (categories.Length == 0)
                        {
                            categories =
                                new[] { "Sin categoría" };
                        }

                        var pubDateText =
                            rssItem.Element("pubDate")?.Value;

                        DateTime.TryParse(
                            pubDateText,
                            out var pubDate);

                        items.Add(new SourceItem
                        {
                            SourceId = source.Id,

                            SourceName = source.Name,

                            SourceType = source.ComponentType,

                            Title = title,

                            Description = description,

                            Url = link,

                            Category = categories,

                            PublishedAt =
                                pubDate == default
                                ? DateTime.UtcNow
                                : pubDate
                        });
                    }

                    return items;
                },
                cacheMinutes);
        }
    }
}
