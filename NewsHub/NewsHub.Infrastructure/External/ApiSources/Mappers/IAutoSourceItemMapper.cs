using Newtonsoft.Json.Linq;
using NewsHub.Application.Common.Models;
using NewsHub.Domain.Entities;

namespace NewsHub.Infrastructure.External.ApiSources.Mappers
{
    public interface IAutoSourceItemMapper
    {
        SourceItem Map(SourceEnt source, JToken obj);
    }
}