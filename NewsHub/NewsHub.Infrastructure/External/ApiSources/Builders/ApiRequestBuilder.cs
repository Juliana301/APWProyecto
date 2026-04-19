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

        public HttpRequestMessage Build(string url, ApiSourceConfig config)
        {
            url = _environmentVariableResolver.Resolve(url) ?? url;

            var requestConfig = config.Request;

            var mergedQueryParams = MergeDictionaries(
                config.QueryParams,
                requestConfig?.QueryParams);

            if (mergedQueryParams != null && mergedQueryParams.Count > 0)
            {
                url = AddQueryString(url, mergedQueryParams);
            }

            var methodText = requestConfig?.Method ?? "GET";

            var request = new HttpRequestMessage(
                new HttpMethod(methodText.ToUpperInvariant()),
                url);

            var mergedHeaders = MergeDictionaries(
                config.Headers,
                requestConfig?.Headers);

            AddHeaders(request, mergedHeaders);

            if (requestConfig != null)
            {
                AddAuthentication(request, requestConfig.Auth);
                AddRequestBody(request, requestConfig);
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

        private string AddQueryString(string url, Dictionary<string, string> queryParams)
        {
            var query = System.Web.HttpUtility.ParseQueryString(string.Empty);

            foreach (var param in queryParams)
            {
                var value = _environmentVariableResolver.Resolve(param.Value);

                if (!string.IsNullOrWhiteSpace(value))
                {
                    query[param.Key] = value;
                }
            }

            var separator = url.Contains("?") ? "&" : "?";

            return url + separator + query.ToString();
        }

        private void AddHeaders(HttpRequestMessage request, Dictionary<string, string>? headers)
        {
            if (headers == null)
                return;

            foreach (var header in headers)
            {
                var value = _environmentVariableResolver.Resolve(header.Value);

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

        private void AddAuthentication(HttpRequestMessage request, ApiAuthConfig? authConfig)
        {
            if (authConfig == null)
                return;

            var authType = authConfig.Type;
            var authValue = _environmentVariableResolver.Resolve(authConfig.Value);

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

        private void AddRequestBody(HttpRequestMessage request, ApiRequestConfig requestConfig)
        {
            if (request.Method == HttpMethod.Get || request.Method == HttpMethod.Head)
                return;

            if (requestConfig.Body == null)
                return;

            string content;

            if (requestConfig.Body is string stringBody)
            {
                content = _environmentVariableResolver.Resolve(stringBody) ?? string.Empty;
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