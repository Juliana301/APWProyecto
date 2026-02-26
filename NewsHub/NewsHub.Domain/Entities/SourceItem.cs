using System;
using System.Collections.Generic;
using System.Text;

namespace NewsHub.Domain.Entities
{
    public class SourceItem
    {
        public int Id { get; private set; }

        public int? SourceId { get; private set; }
        public string? Json { get; private set; }

        public DateTime? CreatedAt { get; private set; }

        public Source? Source { get; private set; }

        private SourceItem() { }

        public SourceItem(int? sourceId, string? json)
        {
            SourceId = sourceId;
            Json = json;
            CreatedAt = DateTime.UtcNow;
        }
    }
}
