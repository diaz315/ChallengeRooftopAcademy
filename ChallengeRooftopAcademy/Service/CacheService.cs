using ChallengeRooftopAcademy.Service.Interfaces;
using ChallengeRooftopAcademy.Util;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace ChallengeRooftopAcademy.Service
{
    public class CacheService : IServiceCache
    {
        public readonly IMemoryCache _memoryCache;

        public CacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public void set(object key, object value)
        {
            var newValue = JsonSerializer.Serialize(value);
            _memoryCache.Set(key, newValue);
        }

        public T get<T>(object key)
        {
            var result = _memoryCache.Get(key);
            return result.ToCast<T>();
        }
    }
}
