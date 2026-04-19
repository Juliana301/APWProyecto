using System.Web;

namespace NewsHub.Infrastructure.External.ApiSources.Helpers
{
    public static class UrlHelper
    {
        public static string AddQueryString(
            string url,
            Dictionary<string, string> queryParams,
            Func<string?, string?>? resolver = null)
        {
            if (queryParams == null || queryParams.Count == 0)
                return url;

            var query = HttpUtility.ParseQueryString(string.Empty);

            foreach (var param in queryParams)
            {
                var value = resolver != null
                    ? resolver(param.Value)
                    : param.Value;

                if (!string.IsNullOrWhiteSpace(value))
                {
                    query[param.Key] = value;
                }
            }

            var separator = url.Contains("?") ? "&" : "?";

            return url + separator + query.ToString();
        }
    }
}