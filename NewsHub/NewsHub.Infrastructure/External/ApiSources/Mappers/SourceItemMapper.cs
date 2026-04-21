using Newtonsoft.Json.Linq;
using NewsHub.Application.Common.Models;
using NewsHub.Domain.Entities;
using NewsHub.Infrastructure.External.ApiSources.Config;
using NewsHub.Infrastructure.External.ApiSources.Helpers;

namespace NewsHub.Infrastructure.External.ApiSources.Mappers
{
    public class SourceItemMapper : ISourceItemMapper
    {
        private readonly IDateParser _dateParser;
        private readonly IUrlNormalizerService _urlNormalizerService;

        public SourceItemMapper(
            IDateParser dateParser, 
            IUrlNormalizerService urlNormalizerService)
        {
            _dateParser = dateParser;
            _urlNormalizerService = urlNormalizerService;
        }

        public SourceItem Map(SourceEnt source, JToken obj, ApiMappingConfig mapping)
        {
            var title = JsonHelper.GetValue(obj, mapping.Title) ?? "Sin título";
            var description = JsonHelper.GetValue(obj, mapping.Description) ?? "";
            var rawUrl = JsonHelper.GetValue(obj, mapping.Url);
            var url = _urlNormalizerService.Normalize(rawUrl);
            var category = JsonHelper.GetValue(obj, mapping.Category);
            var publishedRaw = JsonHelper.GetValue(obj, mapping.PublishedAt);
            var publishedAt = _dateParser.Parse(publishedRaw);

            return new SourceItem
            {
                SourceId = source.Id,
                SourceName = source.Name,
                SourceType = source.ComponentType,
                Title = title,
                Description = description,
                Url = url ?? string.Empty,
                Category = new[] { category ?? "Sin categoría" },
                PublishedAt = publishedAt
            };
        }
    }
}