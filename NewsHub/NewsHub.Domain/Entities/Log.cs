using System;
using System.Collections.Generic;
using System.Text;

namespace NewsHub.Domain.Entities
{
    public class Log
    {
        public int Id { get; private set; }

        public int? UserId { get; private set; }

        public string Action { get; private set; } = null!;
        public string? Entity { get; private set; }
        public int? EntityId { get; private set; }

        public DateTime CreatedAt { get; private set; }

        public User? User { get; private set; }

        private Log() { }

        public Log(string action, string? entity = null, int? entityId = null, int? userId = null)
        {
            Action = action;
            Entity = entity;
            EntityId = entityId;
            UserId = userId;
            CreatedAt = DateTime.UtcNow;
        }
    }
}
