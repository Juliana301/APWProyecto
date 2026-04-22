using Newtonsoft.Json.Linq;
using NewsHub.Application.Common.Models;
using NewsHub.Domain.Entities;
using NewsHub.Infrastructure.External.ApiSources.Config;
using NewsHub.Infrastructure.External.ApiSources.Helpers;
using NewsHub.Infrastructure.External.ApiSources.Mappers;
using NewsHub.Infrastructure.External.ApiSources.Services;

namespace NewsHub.Infrastructure.External.ApiSources.Readers
{
    public class IdPipelineApiReader : IIdPipelineApiReader
    {
        private readonly IApiResponseService _apiResponseService;
        private readonly ISourceItemMapper _sourceItemMapper;

        public IdPipelineApiReader(
            IApiResponseService apiResponseService,
            ISourceItemMapper sourceItemMapper)
        {
            _apiResponseService = apiResponseService;
            _sourceItemMapper = sourceItemMapper;
        }

        public async Task<List<SourceItem>> ReadAsync(
            SourceEnt source,
            ApiSourceConfig config,
            CancellationToken cancellationToken = default)
        {
            var items = new List<SourceItem>();

            if (string.IsNullOrWhiteSpace(config.IdsUrl) ||
                string.IsNullOrWhiteSpace(config.ItemUrlTemplate) ||
                config.Mapping == null)
            {
                return items;
            }

            var token = await _apiResponseService.GetResponseTokenAsync(
                config.IdsUrl,
                config,
                source.Id,
                cancellationToken);

            var idsArray = JsonHelper.GetRootArray(token);

            if (idsArray == null)
                return items;

            var ids = idsArray.Take(config.Limit);
            var semaphore = new SemaphoreSlim(config.MaxConcurrency);

            var tasks = ids.Select(async id =>
            {
                await semaphore.WaitAsync(cancellationToken);

                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var url = config.ItemUrlTemplate.Replace("{id}", id.ToString());

                    var itemToken = await _apiResponseService.GetResponseTokenAsync(
                        url,
                        config,
                        source.Id,
                        cancellationToken);

                    if (itemToken is not JObject obj)
                        return null;

                    var type = obj["type"]?.ToString();
                    if (!string.IsNullOrWhiteSpace(type) &&
                        !type.Equals("story", StringComparison.OrdinalIgnoreCase))
                    {
                        return null;
                    }

                    var link = obj["url"]?.ToString();
                    if (string.IsNullOrWhiteSpace(link))
                        return null;

                    return _sourceItemMapper.Map(source, obj, config.Mapping);
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch
                {
                    return null;
                }
                finally
                {
                    semaphore.Release();
                }
            });

            var results = await Task.WhenAll(tasks);
            return results.Where(x => x != null).Select(x => x!).ToList();
        }
    }
}