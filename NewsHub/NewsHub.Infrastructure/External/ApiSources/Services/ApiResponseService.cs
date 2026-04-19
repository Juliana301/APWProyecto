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
            CancellationToken cancellationToken = default)
        {
            var cacheKey = BuildCacheKey(url, config);

            _logger.LogInformation(
                "Fetching API response. Url: {Url}, CacheMinutes: {CacheMinutes}",
                url,
                config.CacheMinutes);

            return await _cache.GetOrCreateAsync(
                cacheKey,
                async ct =>
                {
                    using var request = _apiRequestBuilder.Build(url, config);

                    _logger.LogDebug(
                        "Sending HTTP request. Method: {Method}, Url: {Url}",
                        request.Method,
                        request.RequestUri);

                    using var response = await _httpClient.SendAsync(request, ct);
                    var content = await response.Content.ReadAsStringAsync(ct);

                    if (!response.IsSuccessStatusCode)
                    {
                        _logger.LogWarning(
                            "API request failed. StatusCode: {StatusCode}, Url: {Url}, Response: {Response}",
                            (int)response.StatusCode,
                            request.RequestUri,
                            Truncate(content, 1000));

                        response.EnsureSuccessStatusCode();
                    }

                    try
                    {
                        var token = JToken.Parse(content);

                        _logger.LogInformation(
                            "API request succeeded. Url: {Url}, StatusCode: {StatusCode}",
                            request.RequestUri,
                            (int)response.StatusCode);

                        return token;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(
                            ex,
                            "Invalid JSON received. Url: {Url}, Response: {Response}",
                            request.RequestUri,
                            Truncate(content, 1000));

                        throw;
                    }
                },
                config.CacheMinutes,
                cancellationToken);
        }

        private static string BuildCacheKey(string url, ApiSourceConfig config)
        {
            var rawKey = $"{url}|{SerializeConfig(config)}";
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