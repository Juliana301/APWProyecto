using System;
using System.Collections.Generic;
using System.Text;

namespace NewsHub.Domain.Entities
{
    public class Source
    {
        public int Id { get; set; }

        public string Url { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string ComponentType { get; set; } = string.Empty;

        public bool RequiresSecret { get; set; }

        public ICollection<SourceItem>? SourceItems { get; set; }
    }
}
