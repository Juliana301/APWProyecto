using System;
using System.Collections.Generic;
using System.Text;

namespace NewsHub.Domain.Entities.CatchError
{
    public class LogErrorEnt
    {
        public int Id { get; private set; }

        public string Origin { get; private set; } = null!;
        public string Message { get; private set; } = null!;
        public string? InnerException { get; private set; }
        public string? StackTrace { get; private set; }

        public DateTime CreatedAt { get; private set; }

        private LogErrorEnt() { } // EF

        public LogErrorEnt(string origin, string message, string? innerException, string? stackTrace)
        {
            Origin = origin;
            Message = message;
            InnerException = innerException;
            StackTrace = stackTrace;
            CreatedAt = DateTime.UtcNow;
        }
    }
}
