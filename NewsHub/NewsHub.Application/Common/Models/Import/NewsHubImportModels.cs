namespace NewsHub.Application.Common.Models.Import
{
    public class NewsHubImportModel
    {
        public NewsHubMeta newsHub { get; set; } = new();
        public SourceImport source { get; set; } = new();
        public ArticleImport article { get; set; } = new();
        public ClassificationImport classification { get; set; } = new();
    }

    public class NewsHubMeta
    {
        public string format { get; set; } = "";
        public DateTime exportedAt { get; set; }
    }

    public class SourceImport
    {
        public int id { get; set; }
        public string name { get; set; } = "";
        public string type { get; set; } = "";
    }

    public class ArticleImport
    {
        public string uniqueId { get; set; } = "";
        public string title { get; set; } = "";
        public string description { get; set; } = "";
        public string url { get; set; } = "";
        public DateTime publishedAt { get; set; }
    }

    public class ClassificationImport
    {
        public string[] categories { get; set; } = Array.Empty<string>();
    }
}