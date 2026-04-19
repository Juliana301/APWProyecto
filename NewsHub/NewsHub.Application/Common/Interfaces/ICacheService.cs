using System;

namespace NewsHub.Application.Common.Interfaces
{
    public interface ICacheService
    {
        Task<T> GetOrCreateAsync<T>(
            string key,
            Func<CancellationToken, Task<T>> factory,
            int minutes,
            CancellationToken cancellationToken = default);

        void Remove(string key);
    }
}