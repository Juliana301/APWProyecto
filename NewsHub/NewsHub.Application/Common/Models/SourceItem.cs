using System;
using System.Collections.Generic;
using System.Text;

namespace NewsHub.Application.Common.Models
{
    public class SourceItem
    {
        public int SourceId { get; set; }

        public string SourceName { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public string[] Category { get; set; } = Array.Empty<string>();

        public string Description { get; set; } = string.Empty;

        public string Url { get; set; } = string.Empty;

        public DateTime PublishedAt { get; set; }
    }
}
