namespace NewsHub.Infrastructure.External.ApiSources.Config
{
    public class ApiRequestConfig
    {
        public string Method { get; set; } = "GET";
        public Dictionary<string, string>? Headers { get; set; }
        public Dictionary<string, string>? QueryParams { get; set; }
        public ApiAuthConfig? Auth { get; set; }
        public object? Body { get; set; }
        public string ContentType { get; set; } = "application/json";
    }
}