using Microsoft.Extensions.Caching.Memory;
using NewsHub.Application.Common.Interfaces;

namespace NewsHub.Infrastructure.Caching
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
            Func<CancellationToken, Task<T>> factory,
            int minutes,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Cache key cannot be null or empty.", nameof(key));

            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            if (minutes <= 0)
                minutes = 5;

            var result = await _cache.GetOrCreateAsync(key, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(minutes);
                return await factory(cancellationToken);
            });

            if (result == null)
                throw new InvalidOperationException("Cache factory returned null.");

            return result;
        }

        public void Remove(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return;

            _cache.Remove(key);
        }
    }
}