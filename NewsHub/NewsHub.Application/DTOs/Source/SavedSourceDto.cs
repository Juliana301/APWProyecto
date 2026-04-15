using System;

namespace NewsHub.Application.DTOs.Source;

public class SavedSourceDto
{
    public int SourceId { get; set; }
    
    public string? id { get; set; }

    public string? source { get; set; }

    public string? type { get; set; }

    public string? title { get; set; }

    public string? description { get; set; }

    public string? date { get; set; }

    public List<string> tags { get; set; }
}
