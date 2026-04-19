using NewsHub.Application.Common.Models;
using NewsHub.Domain.Entities;
using NewsHub.Infrastructure.External.ApiSources.Config;

namespace NewsHub.Infrastructure.External.ApiSources.Readers
{
    public interface IAutoApiReader
    {
        Task<List<SourceItem>> ReadAsync(SourceEnt source, ApiSourceConfig config);
    }
}