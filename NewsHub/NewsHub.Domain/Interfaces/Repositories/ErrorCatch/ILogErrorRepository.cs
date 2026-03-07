using System;
using System.Collections.Generic;
using System.Text;

namespace NewsHub.Domain.Interfaces.Repositories.ErrorCatch
{
    public interface ILogErrorRepository
    {
        Task AddLogErrorAsync(string origin, Exception exception);
    }
}
