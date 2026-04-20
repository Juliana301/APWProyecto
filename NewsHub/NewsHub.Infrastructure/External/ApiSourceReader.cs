using NewsHub.Application.Common.Models;
using NewsHub.Application.Interfaces;
using NewsHub.Domain.Entities;
using NewsHub.Domain.Enums;
using NewsHub.Infrastructure.External.ApiSources.Config;
using NewsHub.Infrastructure.External.ApiSources.Readers;
using Newtonsoft.Json;

namespace NewsHub.Infrastructure.External
{
    public class ApiSourceReader : ISourceReader
    {
        private readonly ISimpleApiReader _simpleApiReader;
        private readonly IIdPipelineApiReader _idPipelineApiReader;
        private readonly IAutoApiReader _autoApiReader;

        public ApiSourceReader(
            ISimpleApiReader simpleApiReader,
            IIdPipelineApiReader idPipelineApiReader,
            IAutoApiReader autoApiReader)
        {
            _simpleApiReader = simpleApiReader;
            _idPipelineApiReader = idPipelineApiReader;
            _autoApiReader = autoApiReader;
        }

        public SourceType Type => SourceType.Api;

        public async Task<List<SourceItem>> ReadAsync(
            SourceEnt source,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(source.ApiConfigJson))
                return new List<SourceItem>();

            try
            {
                var config = JsonConvert.DeserializeObject<ApiSourceConfig>(source.ApiConfigJson);

                if (config == null)
                    return new List<SourceItem>();

                return config.Type switch
                {
                    "Simple" => await _simpleApiReader.ReadAsync(source, config, cancellationToken),

                    "IdPipeline" => await _idPipelineApiReader.ReadAsync(source, config, cancellationToken),

                    "Auto" => await _autoApiReader.ReadAsync(source, config, cancellationToken),

                    _ => new List<SourceItem>()
                };
            }
            catch (OperationCanceledException)
            {
                // importante: no lo tragues
                throw;
            }
            catch
            {
                return new List<SourceItem>();
            }
        }
    }
}