using System;
using System.Collections.Generic;
using System.Text;

namespace NewsHub.Domain.Entities
{
    public class SecretEnt
    {
        public int Id { get; private set; }

        public int? SourceId { get; private set; }

        public string Key { get; private set; } = null!;
        public string Value { get; private set; } = null!;
        public bool IsEncrypted { get; private set; }

        public DateTime CreatedAt { get; private set; }

        public SourceEnt? Source { get; private set; }

        private SecretEnt() { }

        public SecretEnt(string key, string value, bool isEncrypted, int? sourceId = null)
        {
            Key = key;
            Value = value;
            IsEncrypted = isEncrypted;
            SourceId = sourceId;
            CreatedAt = DateTime.UtcNow;
        }
    }
}
