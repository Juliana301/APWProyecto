using Newtonsoft.Json.Linq;

namespace NewsHub.Infrastructure.External.ApiSources.Helpers
{
    public static class JsonHelper
    {
        public static string? GetValue(JToken obj, string? path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return null;

            return obj.SelectToken(path)?.ToString();
        }

        public static JArray? GetRootArray(JToken token, string? rootPath = null)
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
                if (obj["results"] is JArray results) return results;
                if (obj["data"] is JArray data) return data;
                if (obj["items"] is JArray items) return items;
                if (obj["articles"] is JArray articles) return articles;
                if (obj["entries"] is JArray entries) return entries;

                if (obj["response"] is JObject responseObj)
                {
                    if (responseObj["data"] is JArray responseData) return responseData;
                    if (responseObj["items"] is JArray responseItems) return responseItems;
                }

                return FindRootArray(obj);
            }

            return null;
        }

        public static JArray? FindRootArray(JObject json)
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

        public static string? FindValue(JToken obj, params string[] candidates)
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
    }
}