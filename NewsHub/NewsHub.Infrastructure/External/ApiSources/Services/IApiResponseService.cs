using Newtonsoft.Json.Linq;
using NewsHub.Infrastructure.External.ApiSources.Config;

namespace NewsHub.Infrastructure.External.ApiSources.Services
{
    public interface IApiResponseService
    {
        Task<JToken> GetResponseTokenAsync(
            string url,
            ApiSourceConfig config,
            CancellationToken cancellationToken = default);
    }
}