using System;
using System.Collections.Generic;
using System.Text;

namespace NewsHub.Domain.Entities
{
    public class SourceItem
    {
        public int Id { get; set; }

        public int SourceId { get; set; }

        public string Json { get; set; } = "";

        public DateTime CreatedAt { get; set; }

        public Source? Source { get; set; }
    }
}
