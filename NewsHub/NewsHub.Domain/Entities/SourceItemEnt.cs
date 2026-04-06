using System;
using System.Collections.Generic;
using System.Text;

namespace NewsHub.Domain.Entities
{
    public class SourceItemEnt
    {
        public int Id { get; private set; }

        public int? SourceId { get; private set; }
        public string? Json { get; private set; }

        public DateTime? CreatedAt { get; private set; }

        public SourceEnt? Source { get; private set; }
        public ICollection<FavoriteEnt> Favorites { get; private set; } = new List<FavoriteEnt>();

        private SourceItemEnt() { }

        public SourceItemEnt(int? sourceId, string? json)
        {
            SourceId = sourceId;
            Json = json;
            CreatedAt = DateTime.UtcNow;
        }
    }
}
