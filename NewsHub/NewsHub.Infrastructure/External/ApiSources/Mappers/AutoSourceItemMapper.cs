using Newtonsoft.Json.Linq;
using NewsHub.Application.Common.Models;
using NewsHub.Domain.Entities;
using NewsHub.Infrastructure.External.ApiSources.Helpers;

namespace NewsHub.Infrastructure.External.ApiSources.Mappers
{
    public class AutoSourceItemMapper : IAutoSourceItemMapper
    {
        private readonly IDateParser _dateParser;
        private readonly IUrlNormalizerService _urlNormalizerService;

        public AutoSourceItemMapper(
            IDateParser dateParser,
            IUrlNormalizerService urlNormalizerService)
        {
            _dateParser = dateParser;
            _urlNormalizerService = urlNormalizerService;
        }

        public SourceItem Map(SourceEnt source, JToken obj)
        {
            var title = JsonHelper.FindValue(obj, "title", "headline", "name", "headlineText", "subject");
            var description = JsonHelper.FindValue(obj, "description", "summary", "content", "body", "excerpt");
            var rawUrl = JsonHelper.FindValue(obj, "url", "link", "href", "webUrl", "uri");
            var url = _urlNormalizerService.Normalize(rawUrl);
            var category = JsonHelper.FindValue(obj, "category", "section", "tags", "topic", "channel", "genres");
            var date = JsonHelper.FindValue(obj, "publishedAt", "published_at", "published", "date", "createdAt", "updatedAt", "timestamp", "time");

            return new SourceItem
            {
                SourceId = source.Id,
                SourceName = source.Name,
                SourceType = source.ComponentType,
                Title = title ?? "Sin título",
                Description = description ?? "",
                Url = url ?? string.Empty,
                Category = new[] { category ?? "General" },
                PublishedAt = _dateParser.Parse(date)
            };
        }
    }
}