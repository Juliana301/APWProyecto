using NewsHub.Infrastructure.External.ApiSources.Config;

namespace NewsHub.Infrastructure.External.ApiSources.Builders
{
    public interface IApiRequestBuilder
    {
        Task<HttpRequestMessage> BuildAsync(string url, ApiSourceConfig config, int sourceId);
    }
}