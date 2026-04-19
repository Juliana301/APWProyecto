using System;
using Microsoft.Extensions.Caching.Memory;
using NewsHub.Application.Common.Interfaces;

namespace NewsHub.Infrastructure.Catching
{
    public class MemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _cache;

        public MemoryCacheService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public async Task<T> GetOrCreateAsync<T>(
            string key,
            Func<Task<T>> factory,
            int minutes)
        {
            if (_cache.TryGetValue(key, out T cached))
            {
                return cached;
            }

            var result = await factory();

            _cache.Set(
                key,
                result,
                TimeSpan.FromMinutes(minutes));

            return result;
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }
    }
}