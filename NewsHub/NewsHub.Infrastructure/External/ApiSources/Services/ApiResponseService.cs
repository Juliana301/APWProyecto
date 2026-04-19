using NewsHub.Application.Common.Interfaces;
using Newtonsoft.Json.Linq;
using NewsHub.Infrastructure.External.ApiSources.Builders;
using NewsHub.Infrastructure.External.ApiSources.Config;

namespace NewsHub.Infrastructure.External.ApiSources.Services
{
    public class ApiResponseService : IApiResponseService
    {
        private readonly HttpClient _httpClient;
        private readonly ICacheService _cache;
        private readonly IApiRequestBuilder _apiRequestBuilder;

        public ApiResponseService(
            HttpClient httpClient,
            ICacheService cache,
            IApiRequestBuilder apiRequestBuilder)
        {
            _httpClient = httpClient;
            _cache = cache;
            _apiRequestBuilder = apiRequestBuilder;
        }

        public async Task<JToken> GetResponseTokenAsync(string url, ApiSourceConfig config)
        {
            var cacheKey = $"api_cache_{url}_{config.GetHashCode()}";

            return await _cache.GetOrCreateAsync(
                cacheKey,
                async () =>
                {
                    var request = _apiRequestBuilder.Build(url, config);
                    var response = await _httpClient.SendAsync(request);

                    response.EnsureSuccessStatusCode();

                    var json = await response.Content.ReadAsStringAsync();
                    return JToken.Parse(json);
                },
                config.CacheMinutes);
        }
    }
}