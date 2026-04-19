namespace NewsHub.Infrastructure.External.ApiSources.Config
{
    public class ApiSourceConfig
    {
        public string Type { get; set; } = "Auto";
        public string? Root { get; set; }
        public int Limit { get; set; } = 20;
        public int CacheMinutes { get; set; } = 5;
        public int MaxConcurrency { get; set; } = 5;
        public string? IdsUrl { get; set; }
        public string? ItemUrlTemplate { get; set; }
        public ApiMappingConfig? Mapping { get; set; }
        public ApiRequestConfig? Request { get; set; }
        public Dictionary<string, string>? Headers { get; set; }
        public Dictionary<string, string>? QueryParams { get; set; }
    }
}