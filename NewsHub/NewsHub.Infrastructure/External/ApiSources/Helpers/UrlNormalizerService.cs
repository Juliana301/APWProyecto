namespace NewsHub.Infrastructure.External.ApiSources.Helpers
{
    public class UrlNormalizerService : IUrlNormalizerService
    {
        public string? Normalize(string? url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return null;

            url = url.Trim();

            if (url == "#")
                return null;

            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
                return null;

            if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
                return null;

            return uri.AbsoluteUri;
        }

        public bool IsGoogleNewsUrl(string? url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return false;

            return Uri.TryCreate(url, UriKind.Absolute, out var uri)
                && uri.Host.Contains("news.google.com", StringComparison.OrdinalIgnoreCase);
        }

        public bool IsValidAbsoluteUrl(string? url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return false;

            return Uri.TryCreate(url, UriKind.Absolute, out var uri)
                && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
        }
    }
}