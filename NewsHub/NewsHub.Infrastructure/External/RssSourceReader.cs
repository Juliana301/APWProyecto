using System.Xml.Linq;
using Microsoft.Extensions.Logging;
using NewsHub.Application.Common.Interfaces;
using NewsHub.Application.Common.Models;
using NewsHub.Application.Interfaces;
using NewsHub.Domain.Entities;
using NewsHub.Domain.Enums;
using NewsHub.Infrastructure.External.ApiSources.Helpers;

namespace NewsHub.Infrastructure.External
{
    public class RssSourceReader : ISourceReader
    {
        private readonly HttpClient _httpClient;
        private readonly ICacheService _cache;
        private readonly IDateParser _dateParser;
        private readonly ILogger<RssSourceReader> _logger;

        public RssSourceReader(
            HttpClient httpClient,
            ICacheService cache,
            IDateParser dateParser,
            ILogger<RssSourceReader> logger)
        {
            _httpClient = httpClient;
            _cache = cache;
            _dateParser = dateParser;
            _logger = logger;
        }

        public SourceType Type => SourceType.Rss;

        public async Task<List<SourceItem>> ReadAsync(SourceEnt source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (string.IsNullOrWhiteSpace(source.Url))
            {
                _logger.LogWarning(
                    "RSS source skipped because Url is empty. SourceId: {SourceId}, SourceName: {SourceName}",
                    source.Id,
                    source.Name);

                return new List<SourceItem>();
            }

            var cacheKey = $"rss_cache_{source.Id}";
            var cacheMinutes = 5;

            Func<CancellationToken, Task<List<SourceItem>>> factory = async ct =>
            {
                _logger.LogInformation(
                    "Reading RSS source. SourceId: {SourceId}, SourceName: {SourceName}, Url: {Url}",
                    source.Id,
                    source.Name,
                    source.Url);

                var items = new List<SourceItem>();

                string xmlString;
                try
                {
                    xmlString = await _httpClient.GetStringAsync(source.Url, ct);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogWarning(
                        "RSS request was cancelled. SourceId: {SourceId}, Url: {Url}",
                        source.Id,
                        source.Url);

                    throw;
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Error downloading RSS content. SourceId: {SourceId}, Url: {Url}",
                        source.Id,
                        source.Url);

                    return new List<SourceItem>();
                }

                XDocument xml;
                try
                {
                    xml = XDocument.Parse(xmlString);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(
                        ex,
                        "Invalid RSS XML. SourceId: {SourceId}, Url: {Url}",
                        source.Id,
                        source.Url);

                    return new List<SourceItem>();
                }

                var rssItems = xml.Descendants("item").Take(20);

                foreach (var rssItem in rssItems)
                {
                    ct.ThrowIfCancellationRequested();

                    var title = rssItem.Element("title")?.Value?.Trim() ?? "Sin título";

                    var description = rssItem.Element("description")?.Value?.Trim() ?? "Sin descripción";
                    if (description.Length > 500)
                    {
                        description = description[..500];
                    }

                    var link = rssItem.Element("link")?.Value?.Trim() ?? "#";
                    if (string.IsNullOrWhiteSpace(link))
                    {
                        continue;
                    }

                    var categories = rssItem.Elements("category")
                        .Select(c => c.Value?.Trim())
                        .Where(c => !string.IsNullOrWhiteSpace(c))
                        .Cast<string>()
                        .ToArray();

                    if (categories.Length == 0)
                    {
                        categories = new[] { "Sin categoría" };
                    }

                    var pubDateText = rssItem.Element("pubDate")?.Value;
                    var publishedAt = _dateParser.Parse(pubDateText);

                    items.Add(new SourceItem
                    {
                        SourceId = source.Id,
                        SourceName = source.Name,
                        SourceType = source.ComponentType,
                        Title = title,
                        Description = description,
                        Url = link,
                        Category = categories,
                        PublishedAt = publishedAt
                    });
                }

                _logger.LogInformation(
                    "RSS source processed successfully. SourceId: {SourceId}, ItemsCount: {ItemsCount}",
                    source.Id,
                    items.Count);

                return items;
            };

            return await _cache.GetOrCreateAsync<List<SourceItem>>(
                cacheKey,
                factory,
                cacheMinutes,
                CancellationToken.None);
        }
    }
}