using NewsHub.Application.Common.Models;
using NewsHub.Application.Interfaces;
using NewsHub.Domain.Entities;
using NewsHub.Domain.Enums;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        public async Task<List<SourceItem>> ReadAsync(SourceEnt source)
        {
            var items = new List<SourceItem>();

            if (string.IsNullOrWhiteSpace(source.ApiConfigJson))
                return items;

            try
            {
                var config =
                    JObject.Parse(source.ApiConfigJson);

                var type =
                    config["Type"]?.ToString();

                switch (type)
                {
                    case "Simple":
                        items = await ReadSimpleAsync(
                            source,
                            config
                        );
                        break;

                    case "IdPipeline":
                        items = await ReadIdPipelineAsync(
                            source,
                            config
                        );
                        break;
                }
            }
            catch
            {
                // luego logging
            }

            return items;
        }

        // ===============================
        // SIMPLE API
        // ===============================

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

            var json =
                await _httpClient
                    .GetStringAsync(source.Url);

            var obj =
                JObject.Parse(json);

            var elements =
                obj[root]
                ?.Take(20);

            if (elements == null)
                return items;

            foreach (var element in elements)
            {
                items.Add(
                    MapItem(
                        source,
                        element,
                        mapping
                    )
                );
            }

            return items;
        }

        // ===============================
        // ID PIPELINE (HACKER NEWS)
        // ===============================

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

            // 1️⃣ Obtener IDs

            var idsJson =
                await _httpClient
                    .GetStringAsync(idsUrl);

            var ids =
                JArray.Parse(idsJson)
                .Take(limit);

            // 2️⃣ Obtener cada item

            foreach (var id in ids)
            {
                var url =
                    template
                    .Replace("{id}", id.ToString());

                var itemJson =
                    await _httpClient
                        .GetStringAsync(url);

                var obj =
                    JObject.Parse(itemJson);

                items.Add(
                    MapItem(
                        source,
                        obj,
                        mapping
                    )
                );
            }

            return items;
        }

        // ===============================
        // MAPPING
        // ===============================

        private SourceItem MapItem(
            SourceEnt source,
            JToken obj,
            JToken mapping)
        {
            var title =
                GetValue(
                    obj,
                    mapping["Title"]?.ToString()
                )
                ?? "Sin título";

            var description =
                GetValue(
                    obj,
                    mapping["Description"]?.ToString()
                )
                ?? "";

            var url =
                GetValue(
                    obj,
                    mapping["Url"]?.ToString()
                )
                ?? "#";

            var category =
                GetValue(
                    obj,
                    mapping["Category"]?.ToString()
                );

            var publishedRaw =
                GetValue(
                    obj,
                    mapping["PublishedAt"]?.ToString()
                );

            var publishedAt =
                ParseDate(publishedRaw);

            return new SourceItem
            {
                SourceId = source.Id,
                SourceName = source.Name,

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

        // ===============================
        // HELPERS
        // ===============================

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
