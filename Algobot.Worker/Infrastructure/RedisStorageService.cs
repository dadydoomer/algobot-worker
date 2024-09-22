using Algobot.Worker.Configuration;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Algobot.Worker.Infrastructure
{
    public interface IStorageService
    {
        Task SetAsync<T>(string key, T value, TimeSpan expiration);

        bool TryGetValue<T>(string key, out T? value);
    }

    public class RedisStorageService : IStorageService
    {
        private readonly IDistributedCache _cache;

        public RedisStorageService(IDistributedCache cache)
        {
            _cache = cache;
        }

        private static JsonSerializerOptions serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = null,
            WriteIndented = true,
            AllowTrailingCommas = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        public Task SetAsync<T>(string key, T value, TimeSpan expiration)
        {
            return SetAsync(key, value, new DistributedCacheEntryOptions()
                .SetSlidingExpiration(expiration));
        }

        private Task SetAsync<T>(string key, T value, DistributedCacheEntryOptions options)
        {
            var bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(value, serializerOptions));
            return _cache.SetAsync(key, bytes, options);
        }

        public bool TryGetValue<T>(string key, out T? value)
        {
            var val = _cache.Get(key);
            value = default;
            if (val == null) return false;
            value = JsonSerializer.Deserialize<T>(val, serializerOptions);
            return true;
        }
    }
}
