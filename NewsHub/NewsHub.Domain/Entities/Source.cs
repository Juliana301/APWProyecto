using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace NewsHub.Domain.Entities
{
    public class Source
    {
        public int Id { get; private set; }

        public string Url { get; private set; } = null!;
        public string Name { get; private set; } = null!;
        public string? Description { get; private set; }

        public string ComponentType { get; private set; } = null!;
        public bool RequiresSecret { get; private set; }

        public ICollection<SourceItem> Items { get; private set; } = new List<SourceItem>();
        public ICollection<Secret> Secrets { get; private set; } = new List<Secret>();

        private Source() { }

        public Source(string url, string name, string componentType, bool requiresSecret, string? description = null)
        {
            Url = url;
            Name = name;
            ComponentType = componentType;
            RequiresSecret = requiresSecret;
            Description = description;
        }
    }
}
