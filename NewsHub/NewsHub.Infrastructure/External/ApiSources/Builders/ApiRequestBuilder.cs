using System.Net.Http.Headers;
using System.Text;
using NewsHub.Infrastructure.External.ApiSources.Config;
using NewsHub.Infrastructure.External.ApiSources.Helpers;

namespace NewsHub.Infrastructure.External.ApiSources.Builders
{
    public class ApiRequestBuilder : IApiRequestBuilder
    {
        private readonly IEnvironmentVariableResolver _environmentVariableResolver;

        public ApiRequestBuilder(IEnvironmentVariableResolver environmentVariableResolver)
        {
            _environmentVariableResolver = environmentVariableResolver;
        }

        public async Task<HttpRequestMessage> BuildAsync(string url, ApiSourceConfig config, int sourceId)
        {
            url = await _environmentVariableResolver.ResolveAsync(url, sourceId) ?? url;

            var requestConfig = config.Request;

            var mergedQueryParams = MergeDictionaries(
                config.QueryParams,
                requestConfig?.QueryParams);

            if (mergedQueryParams != null && mergedQueryParams.Count > 0)
            {
                var resolvedQueryParams = new Dictionary<string, string>();

                foreach (var item in mergedQueryParams)
                {
                    resolvedQueryParams[item.Key] =
                        await _environmentVariableResolver.ResolveAsync(item.Value, sourceId) ?? item.Value;
                }

                url = UrlHelper.AddQueryString(
                    url,
                    resolvedQueryParams,
                    value => value
                );
            }

            var methodText = requestConfig?.Method ?? "GET";

            var request = new HttpRequestMessage(
                new HttpMethod(methodText.ToUpperInvariant()),
                url);

            var mergedHeaders = MergeDictionaries(
                config.Headers,
                requestConfig?.Headers);

            await AddHeadersAsync(request, mergedHeaders, sourceId);

            if (requestConfig != null)
            {
                await AddAuthenticationAsync(request, requestConfig.Auth, sourceId);
                await AddRequestBodyAsync(request, requestConfig, sourceId);
            }

            return request;
        }

        private static Dictionary<string, string>? MergeDictionaries(
            Dictionary<string, string>? baseDictionary,
            Dictionary<string, string>? overrideDictionary)
        {
            if (baseDictionary == null && overrideDictionary == null)
                return null;

            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (baseDictionary != null)
            {
                foreach (var item in baseDictionary)
                {
                    result[item.Key] = item.Value;
                }
            }

            if (overrideDictionary != null)
            {
                foreach (var item in overrideDictionary)
                {
                    result[item.Key] = item.Value;
                }
            }

            return result;
        }

        private async Task AddHeadersAsync(
            HttpRequestMessage request,
            Dictionary<string, string>? headers,
            int sourceId)
        {
            if (headers == null)
                return;

            foreach (var header in headers)
            {
                var value = await _environmentVariableResolver.ResolveAsync(header.Value, sourceId);

                if (string.IsNullOrWhiteSpace(value))
                    continue;

                if (!request.Headers.TryAddWithoutValidation(header.Key, value))
                {
                    if (request.Content == null)
                    {
                        request.Content = new StringContent(string.Empty);
                    }

                    request.Content.Headers.TryAddWithoutValidation(header.Key, value);
                }
            }
        }

        private async Task AddAuthenticationAsync(
            HttpRequestMessage request,
            ApiAuthConfig? authConfig,
            int sourceId)
        {
            if (authConfig == null)
                return;

            var authType = authConfig.Type;
            var authValue = await _environmentVariableResolver.ResolveAsync(authConfig.Value, sourceId);

            if (string.IsNullOrWhiteSpace(authValue))
                return;

            if (string.Equals(authType, "Bearer", StringComparison.OrdinalIgnoreCase))
            {
                request.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", authValue);
            }
            else if (string.Equals(authType, "Basic", StringComparison.OrdinalIgnoreCase))
            {
                var base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(authValue));

                request.Headers.Authorization =
                    new AuthenticationHeaderValue("Basic", base64);
            }
            else
            {
                request.Headers.Authorization =
                    AuthenticationHeaderValue.Parse(authValue);
            }
        }

        private async Task AddRequestBodyAsync(
            HttpRequestMessage request,
            ApiRequestConfig requestConfig,
            int sourceId)
        {
            if (request.Method == HttpMethod.Get || request.Method == HttpMethod.Head)
                return;

            if (requestConfig.Body == null)
                return;

            string content;

            if (requestConfig.Body is string stringBody)
            {
                content = await _environmentVariableResolver.ResolveAsync(stringBody, sourceId) ?? string.Empty;
            }
            else
            {
                content = System.Text.Json.JsonSerializer.Serialize(requestConfig.Body);
            }

            var contentType = requestConfig.ContentType ?? "application/json";

            request.Content = new StringContent(content, Encoding.UTF8, contentType);
        }
    }
}