using System;

namespace NewsHub.Domain.Entities
{
    public class FavoriteEnt
    {
        public int Id { get; private set; }

        public int UserId { get; private set; }
        public int SourceItemId { get; private set; }

        public DateTime AddedAt { get; private set; }

        public UserEnt User { get; private set; } = null!;
        public SourceItemEnt SourceItem { get; private set; } = null!;

        private FavoriteEnt() { } // EF

        public FavoriteEnt(int userId, int sourceItemId)
        {
            UserId = userId;
            SourceItemId = sourceItemId;
            AddedAt = DateTime.UtcNow;
        }
    }
}