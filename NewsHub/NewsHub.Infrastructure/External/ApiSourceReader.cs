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

        public ApiSourceReader(HttpClient httpClient)
        {
            _httpClient = httpClient;
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
                var config =
                    JObject.Parse(source.ApiConfigJson);

                var readerType =
                    config["Type"]?.ToString();

                items = readerType switch
                {
                    "Simple" =>
                        await ReadSimpleAsync(source, config),

                    "IdPipeline" =>
                        await ReadIdPipelineAsync(source, config),

                    "Auto" =>
                        await ReadAutoAsync(source, config),

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

        private async Task<List<SourceItem>>
            ReadSimpleAsync(
                SourceEnt source,
                JObject config)
        {
            var items = new List<SourceItem>();

            var root =
                config["Root"]?.ToString();

            var mapping =
                config["Mapping"];

            var limit =
                config["Limit"]?.Value<int>()
                ?? 20;

            var json =
                await GetJsonAsync(
                    source.Url,
                    config);

            var elements =
                json[root]
                ?.Take(limit);

            if (elements == null)
                return items;

            foreach (var element in elements)
            {
                var item =
                    MapItem(
                        source,
                        element,
                        mapping);

                items.Add(item);
            }

            return items;
        }

        // =========================================================
        // ID PIPELINE READER
        // =========================================================

        private async Task<List<SourceItem>>
            ReadIdPipelineAsync(
                SourceEnt source,
                JObject config)
        {
            var items =
                new List<SourceItem>();

            var idsUrl =
                config["IdsUrl"]?.ToString();

            var template =
                config["ItemUrlTemplate"]
                ?.ToString();

            var limit =
                config["Limit"]?.Value<int>()
                ?? 20;

            var mapping =
                config["Mapping"];

            if (string.IsNullOrWhiteSpace(idsUrl) ||
                string.IsNullOrWhiteSpace(template))
                return items;

            // Obtener IDs

            var idsArray =
                await GetArrayAsync(
                    idsUrl,
                    config);

            var ids =
                idsArray.Take(limit);

            // Obtener items

            foreach (var id in ids)
            {
                var url =
                    template.Replace(
                        "{id}",
                        id.ToString());

                var obj =
                    await GetJsonAsync(
                        url,
                        config);

                var item =
                    MapItem(
                        source,
                        obj,
                        mapping);

                items.Add(item);
            }

            return items;
        }

        // =========================================================
        // AUTO READER (INTELIGENTE)
        // =========================================================

        private async Task<List<SourceItem>>
            ReadAutoAsync(
                SourceEnt source,
                JObject config)
        {
            var items = new List<SourceItem>();

            var limit =
                config["Limit"]?.Value<int>()
                ?? 20;

            var json =
                await GetJsonAsync(
                    source.Url,
                    config);

            // Detectar automáticamente el array principal

            var root =
                FindRootArray(json);

            if (root == null)
                return items;

            foreach (var obj in root.Take(limit))
            {
                var item =
                    MapAuto(source, obj);

                items.Add(item);
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

        private SourceItem MapAuto(
            SourceEnt source,
            JToken obj)
        {
            var title =
                FindValue(
                    obj,
                    "title",
                    "headline",
                    "name");

            var description =
                FindValue(
                    obj,
                    "description",
                    "summary",
                    "content");

            var url =
                FindValue(
                    obj,
                    "url",
                    "link");

            var category =
                FindValue(
                    obj,
                    "category",
                    "section");

            var date =
                FindValue(
                    obj,
                    "publishedAt",
                    "date",
                    "createdAt",
                    "time");

            return new SourceItem
            {
                SourceId = source.Id,
                SourceName = source.Name,
                SourceType = source.ComponentType,

                Title =
                    title ?? "Sin título",

                Description =
                    description ?? "",

                Url =
                    url ?? "#",

                Category =
                    new[]
                    {
                category
                ?? "General"
                    },

                PublishedAt =
                    ParseDate(date)
            };
        }

        // =========================================================
        // HTTP HELPERS
        // =========================================================

        private async Task<JObject>
            GetJsonAsync(
                string url,
                JObject config)
        {
            var request =
                BuildRequest(url, config);

            var response =
                await _httpClient
                    .SendAsync(request);

            response
                .EnsureSuccessStatusCode();

            var json =
                await response
                    .Content
                    .ReadAsStringAsync();

            return JObject.Parse(json);
        }

        private async Task<JArray>
            GetArrayAsync(
                string url,
                JObject config)
        {
            var request =
                BuildRequest(url, config);

            var response =
                await _httpClient
                    .SendAsync(request);

            response
                .EnsureSuccessStatusCode();

            var json =
                await response
                    .Content
                    .ReadAsStringAsync();

            return JArray.Parse(json);
        }

        // =========================================================
        // REQUEST BUILDER
        // =========================================================

        private HttpRequestMessage
            BuildRequest(
                string url,
                JObject config)
        {
            // =========================
            // Query Params
            // =========================

            var queryParams =
                config["QueryParams"]
                as JObject;

            if (queryParams != null)
            {
                var uriBuilder =
                    new UriBuilder(url);

                var query =
                    System.Web
                    .HttpUtility
                    .ParseQueryString(
                        uriBuilder.Query);

                foreach (var prop in queryParams)
                {
                    var value =
                        ResolveEnvironmentVariable(
                            prop.Value?.ToString());

                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        query[prop.Key] = value;
                    }
                }

                uriBuilder.Query =
                    query.ToString();

                url =
                    uriBuilder.ToString();
            }

            var request =
                new HttpRequestMessage(
                    HttpMethod.Get,
                    url);

            // =========================
            // Headers
            // =========================

            var headers =
                config["Headers"]
                as JObject;

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    var value =
                        ResolveEnvironmentVariable(
                            header.Value?.ToString());

                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        request.Headers.Add(
                            header.Key,
                            value);
                    }
                }
            }

            return request;
        }

        // =========================================================
        // ENVIRONMENT VARIABLE SUPPORT
        // =========================================================

        private string? ResolveEnvironmentVariable(
            string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return value;

            if (value.StartsWith("{{") &&
                value.EndsWith("}}"))
            {
                var envName =
                    value
                        .Replace("{{", "")
                        .Replace("}}", "");

                return Environment
                    .GetEnvironmentVariable(envName);
            }

            return value;
        }

        // =========================================================
        // JSON HELPERS
        // =========================================================

        private string? GetValue(
            JToken obj,
            string? path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return null;

            return obj
                .SelectToken(path)
                ?.ToString();
        }

        // Detectar automáticamente el array root
        private JArray? FindRootArray(
            JObject json)
        {
            foreach (var prop in json.Properties())
            {
                if (prop.Value is JArray arr)
                    return arr;
            }

            return null;
        }

        // Buscar valor automáticamente por nombres comunes

        private string? FindValue(
            JToken obj,
            params string[] candidates)
        {
            foreach (var name in candidates)
            {
                var token =
                    obj.SelectToken($"..{name}");

                if (token != null)
                    return token.ToString();
            }

            return null;
        }

        // =========================================================
        // DATE HELPERS
        // =========================================================

        private DateTime ParseDate(
            string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return DateTime.UtcNow;

            // Unix timestamp

            if (long.TryParse(
                value,
                out var unix))
            {
                return DateTimeOffset
                    .FromUnixTimeSeconds(unix)
                    .UtcDateTime;
            }

            // ISO Date

            if (DateTime.TryParse(
                value,
                out var date))
            {
                return date;
            }

            return DateTime.UtcNow;
        }
    }
}