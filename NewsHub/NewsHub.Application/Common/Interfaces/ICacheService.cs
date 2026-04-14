using System;

namespace NewsHub.Application.Common.Interfaces
{
    public interface ICacheService
    {
        Task<T> GetOrCreateAsync<T>(
        string key,
        Func<Task<T>> factory,
        int minutes);

    void Remove(string key);
    }
}