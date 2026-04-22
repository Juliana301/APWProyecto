using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<ApiResponseService> _logger;

        public ApiResponseService(
            HttpClient httpClient,
            ICacheService cache,
            IApiRequestBuilder apiRequestBuilder,
            ILogger<ApiResponseService> logger)
        {
            _httpClient = httpClient;
            _cache = cache;
            _apiRequestBuilder = apiRequestBuilder;
            _logger = logger;
        }

        public async Task<JToken> GetResponseTokenAsync(
            string url,
            ApiSourceConfig config,
            int sourceId,
            CancellationToken cancellationToken = default)
        {
            var cacheKey = BuildCacheKey(url, config, sourceId);

            _logger.LogInformation(
                "Fetching API response. SourceId: {SourceId}, Url: {Url}, CacheMinutes: {CacheMinutes}",
                sourceId,
                url,
                config.CacheMinutes);

            return await _cache.GetOrCreateAsync(
                cacheKey,
                async ct =>
                {
                    using var request = await _apiRequestBuilder.BuildAsync(url, config, sourceId);

                    _logger.LogDebug(
                        "Sending HTTP request. Method: {Method}, Url: {Url}, SourceId: {SourceId}",
                        request.Method,
                        request.RequestUri,
                        sourceId);

                    using var response = await _httpClient.SendAsync(request, ct);
                    var content = await response.Content.ReadAsStringAsync(ct);

                    if (!response.IsSuccessStatusCode)
                    {
                        _logger.LogWarning(
                            "API request failed. StatusCode: {StatusCode}, Url: {Url}, SourceId: {SourceId}, Response: {Response}",
                            (int)response.StatusCode,
                            request.RequestUri,
                            sourceId,
                            Truncate(content, 1000));

                        response.EnsureSuccessStatusCode();
                    }

                    try
                    {
                        var token = JToken.Parse(content);

                        _logger.LogInformation(
                            "API request succeeded. Url: {Url}, SourceId: {SourceId}, StatusCode: {StatusCode}",
                            request.RequestUri,
                            sourceId,
                            (int)response.StatusCode);

                        return token;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(
                            ex,
                            "Invalid JSON received. Url: {Url}, SourceId: {SourceId}, Response: {Response}",
                            request.RequestUri,
                            sourceId,
                            Truncate(content, 1000));

                        throw;
                    }
                },
                config.CacheMinutes,
                cancellationToken);
        }

        private static string BuildCacheKey(string url, ApiSourceConfig config, int sourceId)
        {
            var rawKey = $"{sourceId}|{url}|{SerializeConfig(config)}";
            var hash = ComputeSha256(rawKey);
            return $"api_cache_{hash}";
        }

        private static string SerializeConfig(ApiSourceConfig config)
        {
            return System.Text.Json.JsonSerializer.Serialize(config);
        }

        private static string ComputeSha256(string input)
        {
            var bytes = Encoding.UTF8.GetBytes(input);
            var hashBytes = SHA256.HashData(bytes);
            return Convert.ToHexString(hashBytes);
        }

        private static string Truncate(string? value, int maxLength)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            return value.Length <= maxLength
                ? value
                : value[..maxLength] + "...";
        }
    }
}