using NewsHub.Application.Common.Models;
using NewsHub.Application.Interfaces;
using NewsHub.Domain.Entities;
using NewsHub.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace NewsHub.Infrastructure.External
{
    public class RssSourceReader : ISourceReader
    {
        private readonly HttpClient _httpClient;

        public RssSourceReader(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public SourceType Type => SourceType.Rss;

        public async Task<List<SourceItem>> ReadAsync(SourceEnt source)
        {
            var items = new List<SourceItem>();

            try
            {
                var xmlString =
                    await _httpClient.GetStringAsync(
                        source.Url);

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
            }
            catch
            {
                // luego puedes agregar logging
            }

            return items;
        }
    }
}
