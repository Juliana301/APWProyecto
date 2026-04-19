using NewsHub.Application.Common.Models;
using NewsHub.Domain.Entities;
using NewsHub.Infrastructure.External.ApiSources.Config;
using NewsHub.Infrastructure.External.ApiSources.Helpers;
using NewsHub.Infrastructure.External.ApiSources.Mappers;
using NewsHub.Infrastructure.External.ApiSources.Services;

namespace NewsHub.Infrastructure.External.ApiSources.Readers
{
    public class SimpleApiReader : ISimpleApiReader
    {
        private readonly IApiResponseService _apiResponseService;
        private readonly ISourceItemMapper _sourceItemMapper;

        public SimpleApiReader(
            IApiResponseService apiResponseService,
            ISourceItemMapper sourceItemMapper)
        {
            _apiResponseService = apiResponseService;
            _sourceItemMapper = sourceItemMapper;
        }

        public async Task<List<SourceItem>> ReadAsync(SourceEnt source, ApiSourceConfig config)
        {
            var items = new List<SourceItem>();
            var limit = config.Limit;
            var root = config.Root;

            var token = await _apiResponseService.GetResponseTokenAsync(source.Url, config);
            var rootArray = JsonHelper.GetRootArray(token, root);

            if (rootArray == null || config.Mapping == null)
                return items;

            foreach (var element in rootArray.Take(limit))
            {
                items.Add(_sourceItemMapper.Map(source, element, config.Mapping));
            }

            return items;
        }
    }
}