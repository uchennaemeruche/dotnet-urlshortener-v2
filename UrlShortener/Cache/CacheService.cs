using System;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;

namespace UrlShortener.Cache
{
    public class CacheService : ICacheService
    {

        private IDatabase _cache;

        private void ConfigureRedis()
        {
            _cache = ConnectionHelper.Connection.GetDatabase();
        }

        public CacheService()
        {
            ConfigureRedis();
        }

       


        public T GetData<T>(string key)
        {
            var cachedData = _cache.StringGet(key);

            if (string.IsNullOrEmpty(cachedData))
            {
                return default(T);
            }
  

            return JsonSerializer.Deserialize<T>(cachedData);
        }

        public bool RemoveData<T>(string key)
        {
            return _cache.KeyDelete(key);
        }

        public bool SetData<T>(string key, T value, DateTimeOffset expirationTime)
        {

            TimeSpan expiryTime = expirationTime.DateTime.Subtract(DateTime.Now);
           

           return _cache.StringSet(key, JsonSerializer.Serialize(value), expiryTime);
        }
    }

}

