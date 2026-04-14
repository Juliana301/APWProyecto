using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Caching.Memory;
using NewsHub.Application.Common.Interfaces;
using NewsHub.Application.Common.Models;
using NewsHub.Application.Interfaces;
using NewsHub.Domain.Entities;
using NewsHub.Domain.Enums;
using Newtonsoft.Json.Linq;

namespace NewsHub.Infrastructure.External
{
    public class ApiSourceReader : ISourceReader
    {
        private readonly HttpClient _httpClient;
        private readonly ICacheService _cache;

        public ApiSourceReader(HttpClient httpClient, ICacheService cache)
        {
            _httpClient = httpClient;
            _cache = cache;
        }

        public SourceType Type => SourceType.Api;

        // =========================================================
        // ENTRY POINT
        // =========================================================

        public async Task<List<SourceItem>> ReadAsync(SourceEnt source)
        {
            var items = new List<SourceItem>();

            if (string.IsNullOrWhiteSpace(source.ApiConfigJson))
                return items;

            try
            {
                var config = JObject.Parse(source.ApiConfigJson);
                var readerType = config["Type"]?.ToString() ?? "Auto";

                items = readerType switch
                {
                    "Simple" => await ReadSimpleAsync(source, config),
                    "IdPipeline" => await ReadIdPipelineAsync(source, config),
                    "Auto" => await ReadAutoAsync(source, config),
                    _ => new List<SourceItem>()
                };
            }
            catch (Exception ex)
            {
                // TODO: logging
                // _logger.LogError(ex, "Error reading API source");
            }

            return items;
        }

        // =========================================================
        // SIMPLE READER
        // =========================================================

        private async Task<List<SourceItem>> ReadSimpleAsync(SourceEnt source, JObject config)
        {
            var items = new List<SourceItem>();
            var root = config["Root"]?.ToString();
            var mapping = config["Mapping"];
            var limit = config["Limit"]?.Value<int>() ?? 20;

            var token = await GetResponseTokenAsync(source.Url, config);
            var rootArray = GetRootArray(token, root);

            if (rootArray == null)
                return items;

            foreach (var element in rootArray.Take(limit))
            {
                var item = MapItem(source, element, mapping);
                items.Add(item);
            }

            return items;
        }

        // =========================================================
        // ID PIPELINE READER
        // =========================================================

        private async Task<List<SourceItem>> ReadIdPipelineAsync(SourceEnt source, JObject config)
        {
            var items = new List<SourceItem>();
            var idsUrl = config["IdsUrl"]?.ToString();
            var template = config["ItemUrlTemplate"]?.ToString();
            var limit = config["Limit"]?.Value<int>() ?? 20;
            var mapping = config["Mapping"];

            if (string.IsNullOrWhiteSpace(idsUrl) || string.IsNullOrWhiteSpace(template))
                return items;

            var idsArray = await GetArrayAsync(idsUrl, config);
            var ids = idsArray.Take(limit);

            var semaphore = new SemaphoreSlim(5);
            var tasks = ids.Select(async id =>
            {
                await semaphore.WaitAsync();
                try
                {
                    var url = template.Replace("{id}", id.ToString());

                    try
                    {
                        var token = await GetResponseTokenAsync(url, config);
                        if (token is not JObject obj)
                            return null;

                        var type = obj["type"]?.ToString();
                        if (!string.IsNullOrWhiteSpace(type) && !type.Equals("story", StringComparison.OrdinalIgnoreCase))
                            return null;

                        var link = obj["url"]?.ToString();
                        if (string.IsNullOrWhiteSpace(link))
                            return null;

                        return MapItem(source, obj, mapping);
                    }
                    catch
                    {
                        return null;
                    }
                }
                finally
                {
                    semaphore.Release();
                }
            });

            var results = await Task.WhenAll(tasks);
            return results.Where(x => x != null).Select(x => x!).ToList();
        }

        // =========================================================
        // AUTO READER (INTELIGENTE)
        // =========================================================

        private async Task<List<SourceItem>> ReadAutoAsync(SourceEnt source, JObject config)
        {
            var items = new List<SourceItem>();
            var limit = config["Limit"]?.Value<int>() ?? 20;
            var rootPath = config["Root"]?.ToString();

            var token = await GetResponseTokenAsync(source.Url, config);
            var rootArray = GetRootArray(token, rootPath);

            if (rootArray == null)
                return items;

            foreach (var itemObj in rootArray.Take(limit))
            {
                items.Add(MapAuto(source, itemObj));
            }

            return items;
        }

        // =========================================================
        // MAPPING
        // =========================================================

        private SourceItem MapItem(
            SourceEnt source,
            JToken obj,
            JToken mapping)
        {
            var title =
                GetValue(
                    obj,
                    mapping["Title"]?.ToString())
                ?? "Sin título";

            var description =
                GetValue(
                    obj,
                    mapping["Description"]?.ToString())
                ?? "";

            var url =
                GetValue(
                    obj,
                    mapping["Url"]?.ToString())
                ?? "#";

            var category =
                GetValue(
                    obj,
                    mapping["Category"]?.ToString());

            var publishedRaw =
                GetValue(
                    obj,
                    mapping["PublishedAt"]?.ToString());

            var publishedAt =
                ParseDate(publishedRaw);

            return new SourceItem
            {
                SourceId = source.Id,
                SourceName = source.Name,
                SourceType = source.ComponentType,

                Title = title,
                Description = description,
                Url = url,

                Category =
                    new[]
                    {
                        category
                        ?? "Sin categoría"
                    },

                PublishedAt = publishedAt
            };
        }

        // =========================================================
        // AUTO MAPPING
        // =========================================================

        private SourceItem MapAuto(SourceEnt source, JToken obj)
        {
            var title = FindValue(obj, "title", "headline", "name", "headlineText", "subject");
            var description = FindValue(obj, "description", "summary", "content", "body", "excerpt");
            var url = FindValue(obj, "url", "link", "href", "webUrl", "uri");
            var category = FindValue(obj, "category", "section", "tags", "topic", "channel", "genres");
            var date = FindValue(obj, "publishedAt", "published_at", "published", "date", "createdAt", "updatedAt", "timestamp", "time");

            return new SourceItem
            {
                SourceId = source.Id,
                SourceName = source.Name,
                SourceType = source.ComponentType,
                Title = title ?? "Sin título",
                Description = description ?? "",
                Url = url ?? "#",
                Category = new[] { category ?? "General" },
                PublishedAt = ParseDate(date)
            };
        }

        // =========================================================
        // HTTP HELPERS
        // =========================================================

        private async Task<JToken> GetResponseTokenAsync(
            string url,
            JObject config)
        {
            var cacheMinutes =
                config["CacheMinutes"]?.Value<int>()
                ?? 5;

            var cacheKey =
                $"api_cache_{url}_{config.ToString().GetHashCode()}";

            return await _cache.GetOrCreateAsync(
                cacheKey,
                async () =>
                {
                    var request =
                        BuildRequest(url, config);

                    var response =
                        await _httpClient.SendAsync(request);

                    response.EnsureSuccessStatusCode();

                    var json =
                        await response.Content
                            .ReadAsStringAsync();

                    return JToken.Parse(json);
                },
                cacheMinutes);
        }

        private async Task<JObject> GetJsonAsync(string url, JObject config)
        {
            var token = await GetResponseTokenAsync(url, config);
            return token as JObject ?? throw new InvalidOperationException("Response JSON is not an object.");
        }

        private async Task<JArray> GetArrayAsync(string url, JObject config)
        {
            var token = await GetResponseTokenAsync(url, config);
            var array = GetRootArray(token, null);
            return array ?? throw new InvalidOperationException("Response JSON does not contain an array.");
        }

        // =========================================================
        // REQUEST BUILDER
        // =========================================================

        private HttpRequestMessage BuildRequest(string url, JObject config)
        {
            var requestConfig =
        config["Request"] as JObject;

            // 🔥 IMPORTANTE
            url =
                ResolveEnvironmentVariable(url);

            var queryParams =
                MergeObjects(
                    config["QueryParams"] as JObject,
                    requestConfig?["QueryParams"] as JObject);

            if (queryParams != null)
                url =
                    AddQueryString(
                        url,
                        queryParams);

            var methodText =
                requestConfig?["Method"]?.ToString()
                ?? "GET";

            var request =
                new HttpRequestMessage(
                    new HttpMethod(
                        methodText.ToUpperInvariant()),
                    url);

            var headers =
                MergeObjects(
                    config["Headers"] as JObject,
                    requestConfig?["Headers"] as JObject);

            AddHeaders(request, headers);

            if (requestConfig != null)
            {
                AddAuthentication(
                    request,
                    requestConfig["Auth"]);

                AddRequestBody(
                    request,
                    requestConfig);
            }

            return request;
        }

        private static JObject? MergeObjects(JObject? baseObject, JObject? overrideObject)
        {
            if (baseObject == null && overrideObject == null)
                return null;

            var result = new JObject();

            if (baseObject != null)
            {
                foreach (var prop in baseObject)
                    result[prop.Key] = prop.Value;
            }

            if (overrideObject != null)
            {
                foreach (var prop in overrideObject)
                    result[prop.Key] = prop.Value;
            }

            return result;
        }

        private string AddQueryString(string url, JObject queryParams)
        {
            var query =
                System.Web.HttpUtility.ParseQueryString("");

            foreach (var param in queryParams)
            {
                var value =
                    ResolveEnvironmentVariable(
                        param.Value?.ToString());

                if (!string.IsNullOrWhiteSpace(value))
                {
                    query[param.Key] =
                        value;
                }
            }

            var separator =
                url.Contains("?") ? "&" : "?";

            return url +
                   separator +
                   query.ToString();
        }

        private void AddHeaders(HttpRequestMessage request, JObject? headers)
        {
            if (headers == null)
                return;

            foreach (var header in headers)
            {
                var value = ResolveEnvironmentVariable(header.Value?.ToString());
                if (string.IsNullOrWhiteSpace(value))
                    continue;

                if (!request.Headers.TryAddWithoutValidation(header.Key, value) && request.Content != null)
                {
                    request.Content.Headers.TryAddWithoutValidation(header.Key, value);
                }
            }
        }

        private void AddAuthentication(HttpRequestMessage request, JToken? authToken)
        {
            if (authToken == null)
                return;

            if (authToken is JObject authObj)
            {
                var authType = authObj["Type"]?.ToString();
                var authValue = ResolveEnvironmentVariable(authObj["Value"]?.ToString());
                if (string.IsNullOrWhiteSpace(authValue))
                    return;

                if (authType?.Equals("Bearer", StringComparison.OrdinalIgnoreCase) == true)
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authValue);
                }
                else if (authType?.Equals("Basic", StringComparison.OrdinalIgnoreCase) == true)
                {
                    var base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(authValue));
                    request.Headers.Authorization = new AuthenticationHeaderValue("Basic", base64);
                }
                else
                {
                    request.Headers.Authorization = AuthenticationHeaderValue.Parse(authValue);
                }
            }
            else
            {
                var authValue = ResolveEnvironmentVariable(authToken.ToString());
                if (!string.IsNullOrWhiteSpace(authValue))
                    request.Headers.Authorization = AuthenticationHeaderValue.Parse(authValue);
            }
        }

        private void AddRequestBody(HttpRequestMessage request, JObject requestConfig)
        {
            if (request.Method == HttpMethod.Get || request.Method == HttpMethod.Head)
                return;

            var bodyToken = requestConfig["Body"];
            if (bodyToken == null)
                return;

            string content;
            if (bodyToken.Type == JTokenType.Object || bodyToken.Type == JTokenType.Array)
            {
                content = bodyToken.ToString();
            }
            else
            {
                content = ResolveEnvironmentVariable(bodyToken.ToString()) ?? string.Empty;
            }

            var contentType = requestConfig["ContentType"]?.ToString() ?? "application/json";
            request.Content = new StringContent(content, Encoding.UTF8, contentType);
        }

        // =========================================================
        // ENVIRONMENT VARIABLE SUPPORT
        // =========================================================

        private string? ResolveEnvironmentVariable(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return value;

            var matches =
                System.Text.RegularExpressions.Regex.Matches(
                    value,
                    @"\{\{(.*?)\}\}");

            foreach (System.Text.RegularExpressions.Match match in matches)
            {
                var varName =
                    match.Groups[1].Value;

                var envValue =
                    Environment.GetEnvironmentVariable(varName);

                if (!string.IsNullOrWhiteSpace(envValue))
                {
                    value =
                        value.Replace(
                            match.Value,
                            envValue);
                }
            }

            return value;
        }

        // =========================================================
        // JSON HELPERS
        // =========================================================

        private string? GetValue(JToken obj, string? path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return null;

            return obj.SelectToken(path)?.ToString();
        }

        // Detectar automáticamente el array root
        private JArray? GetRootArray(JToken token, string? rootPath = null)
        {
            if (!string.IsNullOrWhiteSpace(rootPath))
            {
                var selected = token.SelectToken(rootPath);
                if (selected is JArray selectedArray)
                    return selectedArray;

                if (selected is JObject selectedObject)
                    return FindRootArray(selectedObject);
            }

            if (token is JArray array)
                return array;

            if (token is JObject obj)
            {
                if (obj["results"] is JArray results)
                    return results;
                if (obj["data"] is JArray data)
                    return data;
                if (obj["items"] is JArray items)
                    return items;
                if (obj["articles"] is JArray articles)
                    return articles;
                if (obj["entries"] is JArray entries)
                    return entries;
                if (obj["response"] is JObject responseObj)
                {
                    if (responseObj["data"] is JArray responseData)
                        return responseData;
                    if (responseObj["items"] is JArray responseItems)
                        return responseItems;
                }

                return FindRootArray(obj);
            }

            return null;
        }

        private JArray? FindRootArray(JObject json)
        {
            foreach (var prop in json.Properties())
            {
                if (prop.Value is JArray arr)
                    return arr;
            }

            foreach (var prop in json.Properties())
            {
                if (prop.Value is JObject nested)
                {
                    var nestedArray = FindRootArray(nested);
                    if (nestedArray != null)
                        return nestedArray;
                }
            }

            return null;
        }

        // Buscar valor automáticamente por nombres comunes

        private string? FindValue(JToken obj, params string[] candidates)
        {
            foreach (var name in candidates)
            {
                if (string.IsNullOrWhiteSpace(name))
                    continue;

                if (name.Contains('.') || name.Contains('['))
                {
                    var token = obj.SelectToken(name);
                    if (token != null)
                        return token.ToString();
                }

                if (obj is JContainer container)
                {
                    var property = container
                        .DescendantsAndSelf()
                        .OfType<JProperty>()
                        .FirstOrDefault(p => string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase));

                    if (property != null)
                        return property.Value.ToString();
                }
            }

            return null;
        }

        // =========================================================
        // DATE HELPERS
        // =========================================================

        private DateTime ParseDate(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return DateTime.UtcNow;

            var text = value.Trim();

            if (long.TryParse(text, out var unix))
            {
                if (text.Length >= 13)
                    return DateTimeOffset.FromUnixTimeMilliseconds(unix).UtcDateTime;

                return DateTimeOffset.FromUnixTimeSeconds(unix).UtcDateTime;
            }

            if (DateTimeOffset.TryParse(text, out var offset))
                return offset.UtcDateTime;

            if (DateTime.TryParse(text, out var date))
                return DateTime.SpecifyKind(date, DateTimeKind.Utc);

            return DateTime.UtcNow;
        }
    }
}