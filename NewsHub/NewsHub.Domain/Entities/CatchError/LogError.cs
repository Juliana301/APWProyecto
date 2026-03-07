using System;
using System.Collections.Generic;
using System.Text;

namespace NewsHub.Domain.Entities.CatchError
{
    public class LogError
    {
        public int Id { get; private set; }

        public string Origin { get; private set; } = null!;
        public string Message { get; private set; } = null!;
        public string? InnerException { get; private set; }
        public string? StackTrace { get; private set; }

        public DateTime CreatedAt { get; private set; }

        private LogError() { } // EF

        public LogError(string origin, string message, string? innerException, string? stackTrace)
        {
            Origin = origin;
            Message = message;
            InnerException = innerException;
            StackTrace = stackTrace;
            CreatedAt = DateTime.UtcNow;
        }
    }
}
