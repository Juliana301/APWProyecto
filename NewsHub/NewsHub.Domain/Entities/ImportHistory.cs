using System;
using System.Collections.Generic;
using System.Text;

namespace NewsHub.Domain.Entities
{
    public class ImportHistory
    {
        public int Id { get; private set; }

        public string? SourceName { get; private set; }
        public int ItemsImported { get; private set; }

        public bool Success { get; private set; }
        public string? ErrorMessage { get; private set; }

        public DateTime ImportedAt { get; private set; }

        private ImportHistory() { }

        public ImportHistory(string? sourceName, int itemsImported, bool success, string? errorMessage = null)
        {
            SourceName = sourceName;
            ItemsImported = itemsImported;
            Success = success;
            ErrorMessage = errorMessage;
            ImportedAt = DateTime.UtcNow;
        }
    }
}
