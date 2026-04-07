using NewsHub.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
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
            if (string.IsNullOrWhiteSpace(apiConfigJson))
            {
                ApiConfigJson = null;
                return;
            }

            try
            {
                var json = JsonNode
                    .Parse(apiConfigJson)?
                    .AsObject();

                if (json == null)
                    throw new ArgumentException(
                        "El ApiConfigJson debe ser un objeto JSON."
                    );

                // =========================
                // TYPE obligatorio
                // =========================

                var typeString =
                    json["Type"]?.ToString()
                    ?? "Auto";

                if (!Enum.TryParse<ApiConfigType>(
                    typeString,
                    ignoreCase: true,
                    out var type))
                {
                    throw new ArgumentException(
                        $"Type '{typeString}' no es válido."
                    );
                }

                // =========================
                // VALIDACIÓN SEGÚN TYPE
                // =========================

                switch (type)
                {
                    case ApiConfigType.Simple:

                        if (json["Root"] == null)
                            throw new ArgumentException(
                                "Simple requiere 'Root'."
                            );

                        if (json["Mapping"] == null)
                            throw new ArgumentException(
                                "Simple requiere 'Mapping'."
                            );

                        break;

                    case ApiConfigType.IdPipeline:

                        if (json["IdsUrl"] == null)
                            throw new ArgumentException(
                                "IdPipeline requiere 'IdsUrl'."
                            );

                        if (json["ItemUrlTemplate"] == null)
                            throw new ArgumentException(
                                "IdPipeline requiere 'ItemUrlTemplate'."
                            );

                        if (json["Mapping"] == null)
                            throw new ArgumentException(
                                "IdPipeline requiere 'Mapping'."
                            );

                        break;

                    case ApiConfigType.Auto:

                        // Auto no requiere Root ni Mapping
                        // Solo validamos que tenga Type

                        break;
                }

                // =========================
                // VALIDAR Mapping básico
                // =========================

                // Solo validar mapping si existe

                if (json["Mapping"] != null)
                {
                    var mapping =
                        json["Mapping"]?.AsObject();

                    if (mapping == null)
                        throw new ArgumentException(
                            "Mapping debe ser un objeto."
                        );

                    if (string.IsNullOrWhiteSpace(
                        mapping["Title"]?.ToString()))
                    {
                        throw new ArgumentException(
                            "Mapping requiere 'Title'."
                        );
                    }

                    if (string.IsNullOrWhiteSpace(
                        mapping["Url"]?.ToString()))
                    {
                        throw new ArgumentException(
                            "Mapping requiere 'Url'."
                        );
                    }
                }
            }
            catch (JsonException)
            {
                throw new ArgumentException(
                    "El ApiConfigJson no contiene un JSON válido."
                );
            }

            ApiConfigJson = apiConfigJson;
        }

    }
}
