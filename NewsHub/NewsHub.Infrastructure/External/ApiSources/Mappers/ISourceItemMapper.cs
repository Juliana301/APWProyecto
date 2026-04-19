using Newtonsoft.Json.Linq;
using NewsHub.Application.Common.Models;
using NewsHub.Domain.Entities;
using NewsHub.Infrastructure.External.ApiSources.Config;

namespace NewsHub.Infrastructure.External.ApiSources.Mappers
{
    public interface ISourceItemMapper
    {
        SourceItem Map(SourceEnt source, JToken obj, ApiMappingConfig mapping);
    }
}