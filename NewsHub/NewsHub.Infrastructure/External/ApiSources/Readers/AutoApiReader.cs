using NewsHub.Application.Common.Models;
using NewsHub.Domain.Entities;
using NewsHub.Infrastructure.External.ApiSources.Config;
using NewsHub.Infrastructure.External.ApiSources.Helpers;
using NewsHub.Infrastructure.External.ApiSources.Mappers;
using NewsHub.Infrastructure.External.ApiSources.Services;

namespace NewsHub.Infrastructure.External.ApiSources.Readers
{
    public class AutoApiReader : IAutoApiReader
    {
        private readonly IApiResponseService _apiResponseService;
        private readonly IAutoSourceItemMapper _autoSourceItemMapper;

        public AutoApiReader(
            IApiResponseService apiResponseService,
            IAutoSourceItemMapper autoSourceItemMapper)
        {
            _apiResponseService = apiResponseService;
            _autoSourceItemMapper = autoSourceItemMapper;
        }

        public async Task<List<SourceItem>> ReadAsync(
            SourceEnt source,
            ApiSourceConfig config,
            CancellationToken cancellationToken = default)
        {
            var items = new List<SourceItem>();

            var token = await _apiResponseService.GetResponseTokenAsync(
                source.Url,
                config,
                cancellationToken);

            var rootArray = JsonHelper.GetRootArray(token, config.Root);

            if (rootArray == null)
                return items;

            foreach (var itemObj in rootArray.Take(config.Limit))
            {
                cancellationToken.ThrowIfCancellationRequested();
                items.Add(_autoSourceItemMapper.Map(source, itemObj));
            }

            return items;
        }
    }
}