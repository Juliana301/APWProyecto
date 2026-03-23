using NewsHub.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Text.Json.Nodes;

namespace NewsHub.Domain.Entities
{
    public class SourceEnt
    {
        public int Id { get; private set; }

        public string Url { get; private set; } = null!;
        public string Name { get; private set; } = null!;
        public string? Description { get; private set; }

        public SourceType ComponentType { get; private set; }
        public bool RequiresSecret { get; private set; }

        // ApiConfigJson para almacenar la configuración específica de la API para cada fuente.
        public string? ApiConfigJson { get; private set; }

        public ICollection<SourceItemEnt> Items { get; private set; } = new List<SourceItemEnt>();
        public ICollection<SecretEnt> Secrets { get; private set; } = new List<SecretEnt>();

        private SourceEnt() { }

        public SourceEnt(string url, string name, SourceType componentType, bool requiresSecret, string? description = null)
        {
            Url = url;
            Name = name;
            ComponentType = componentType;
            RequiresSecret = requiresSecret;
            Description = description;
        }

        public void Update(string url, string name, SourceType componentType, bool requiresSecret, string? description = null)
        {
            Url = url;
            Name = name;
            ComponentType = componentType;
            RequiresSecret = requiresSecret;
            Description = description;
        }

        public void SetApiConfig(string? apiConfigJson)
        {
            if (!string.IsNullOrWhiteSpace(apiConfigJson))
            {
                JsonObject.Parse(apiConfigJson);
            }

            ApiConfigJson = apiConfigJson;
        }
    }
}
