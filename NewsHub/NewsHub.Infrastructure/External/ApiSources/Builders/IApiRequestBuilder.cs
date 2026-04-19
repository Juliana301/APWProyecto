using NewsHub.Infrastructure.External.ApiSources.Config;

namespace NewsHub.Infrastructure.External.ApiSources.Builders
{
    public interface IApiRequestBuilder
    {
        HttpRequestMessage Build(string url, ApiSourceConfig config);
    }
}