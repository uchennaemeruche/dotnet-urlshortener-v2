using System;

namespace UrlShortener.Cache
{
    public class CacheService : ICacheService
    {
        public CacheService()
        {
           
        }


        public T GetData<T>(string key)
        {
            throw new NotImplementedException();
        }

        public bool RemoveData<T>(string key)
        {
            throw new NotImplementedException();
        }

        public bool SetData<T>(string key, T value, DateTimeOffset? expirationTime)
        {
            throw new NotImplementedException();
        }
    }

}

