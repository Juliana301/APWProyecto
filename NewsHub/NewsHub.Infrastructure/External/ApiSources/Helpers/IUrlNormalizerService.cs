namespace NewsHub.Infrastructure.External.ApiSources.Helpers
{
    public interface IUrlNormalizerService
    {
        string? Normalize(string? url);
        bool IsGoogleNewsUrl(string? url);
        bool IsValidAbsoluteUrl(string? url);
    }
}