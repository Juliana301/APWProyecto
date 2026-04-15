using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using NewsHub.Domain.Enums;

namespace NewsHub.Application.Common.Models
{
    public class SourceItem
    {
        public int SourceId { get; set; }

        public int Id { get; set; }

        public string UniqueId
        {
            get
            {
                using var sha =
                    SHA256.Create();

                var bytes =
                    sha.ComputeHash(
                        Encoding.UTF8.GetBytes(Url)
                    );

                var hash =
                    Convert.ToHexString(bytes);

                return $"{SourceId}_{hash}";
            }
        }
        public string SourceName { get; set; } = string.Empty;

        public SourceType SourceType { get; set; }

        public string Title { get; set; } = string.Empty;

        public string[] Category { get; set; } = Array.Empty<string>();

        public string Description { get; set; } = string.Empty;

        public string Url { get; set; } = string.Empty;

        public DateTime PublishedAt { get; set; }
    }
}
