using Newtonsoft.Json.Linq;
using NewsHub.Application.Common.Models;
using NewsHub.Domain.Entities;
using NewsHub.Infrastructure.External.ApiSources.Helpers;

namespace NewsHub.Infrastructure.External.ApiSources.Mappers
{
    public class AutoSourceItemMapper : IAutoSourceItemMapper
    {
        private readonly IDateParser _dateParser;

        public AutoSourceItemMapper(IDateParser dateParser)
        {
            _dateParser = dateParser;
        }

        public SourceItem Map(SourceEnt source, JToken obj)
        {
            var title = JsonHelper.FindValue(obj, "title", "headline", "name", "headlineText", "subject");
            var description = JsonHelper.FindValue(obj, "description", "summary", "content", "body", "excerpt");
            var url = JsonHelper.FindValue(obj, "url", "link", "href", "webUrl", "uri");
            var category = JsonHelper.FindValue(obj, "category", "section", "tags", "topic", "channel", "genres");
            var date = JsonHelper.FindValue(obj, "publishedAt", "published_at", "published", "date", "createdAt", "updatedAt", "timestamp", "time");

            return new SourceItem
            {
                SourceId = source.Id,
                SourceName = source.Name,
                SourceType = source.ComponentType,
                Title = title ?? "Sin título",
                Description = description ?? "",
                Url = url ?? "#",
                Category = new[] { category ?? "General" },
                PublishedAt = _dateParser.Parse(date)
            };
        }
    }
}