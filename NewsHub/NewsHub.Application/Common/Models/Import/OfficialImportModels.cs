using System;

namespace NewsHub.Application.Common.Models.Import
{
    public class OfficialImportModel
    {
        public string? schemaVersion { get; set; }

        public DateTime exportedAt { get; set; }

        public SourceImport? source { get; set; }

        public NormalizedImport? normalized { get; set; }

        public RawImport? raw { get; set; }
    }

    public class SourceImport
    {
        public string? id { get; set; }

        public string? name { get; set; }

        public string? type { get; set; }

        public string? url { get; set; }

        public bool requiresSecret { get; set; }
    }

    public class NormalizedImport
    {
        public string? id { get; set; }

        public string? externalId { get; set; }

        public string? title { get; set; }

        public string? content { get; set; }

        public string? summary { get; set; }

        public DateTime publishedAt { get; set; }

        public string? url { get; set; }

        public string? author { get; set; }

        public string? language { get; set; }

        public CategoryImport? category { get; set; }
    }

    public class CategoryImport
    {
        public string? primary { get; set; }

        public List<string> secondary { get; set; } = new();
    }

    public class RawImport
    {
        public string? format { get; set; }

        public object? data { get; set; }
    }

}