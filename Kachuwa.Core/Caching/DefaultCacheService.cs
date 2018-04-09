using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace Kachuwa.Caching
{
    public class DefaultCacheService : ICacheService
    {
        private readonly MemoryCache _cache;
        private readonly object _lock = new Object();
        private const int ExpireInSeconds = 600;//10 min

        public DefaultCacheService()
        {
            _cache = new MemoryCache(new MemoryCacheOptions()
            {
                
            });
        }


        public void Dispose()
        {
            _cache.Dispose();
        }

        public T Get<T>(string key)
        {
            lock (_lock)
            {
                var cacheObj = _cache.Get(key);
                return (T)cacheObj;
            }

        }
        public T Get<T>(string key, Func<T> dataFactory)
        {
            return Get<T>(key, ExpireInSeconds, dataFactory);

        }

        public T Get<T>(string key, TimeSpan cachingLife, Func<T> dataFactory)
        {
            lock (_lock)
            {
                var cacheObj = _cache.Get(key);
                if (cacheObj == null)
                {
                    cacheObj = dataFactory();
                    _cache.Set(key, cacheObj, new DateTimeOffset(DateTime.Now, cachingLife));
                }
                return (T)cacheObj;
            }
        }

        public T Get<T>(string key, int expireInSeconds, Func<T> dataFactory)
        {
            lock (_lock)
            {
                var cacheObj = _cache.Get(key);
                if (cacheObj == null)
                {
                    cacheObj = dataFactory();
                    _cache.Set(key, cacheObj, new DateTimeOffset(
                        DateTime.UtcNow.AddSeconds(expireInSeconds)
                    ));
                }
                return (T)cacheObj;
            }
        }

        public Task<T> GetAsync<T>(string key, Func<Task<T>> dataFactory)
        {
            return GetAsync<T>(key, ExpireInSeconds, dataFactory);
        }

        public async Task<T> GetAsync<T>(string key, int expireInSeconds, Func<Task<T>> dataFactory)
        {
            var cacheObj = await _cache.GetOrCreateAsync(key, async entry =>
            {
                entry.AbsoluteExpiration = new DateTimeOffset(
                    DateTime.UtcNow.AddSeconds(expireInSeconds)
                );
                //entry.Value = dataFactory();
                return await dataFactory();
            });
            return (T)cacheObj;
        }

        public async Task<T> GetAsync<T>(string key, Func<Task<T>> dataFactory, TimeSpan cachingLife)
        {
            var cacheObj = await _cache.GetOrCreateAsync(key, async entry =>
            {
                entry.AbsoluteExpiration = new DateTimeOffset(DateTime.Now, cachingLife);
                //entry.Value = dataFactory();
                return await dataFactory();
            });
            return (T)cacheObj;
        }

        public void Remove(string key)
        {
            lock (_lock)
            {
                _cache.Remove(key);
            }
        }
        public void Flush()
        {

            //var cacheItems = _cache.

            //foreach (KeyValuePair<String, Object> a in cacheItems)
            //{
            //    lock (_lock)
            //    {
            //        _cache.Remove(a.Key);
            //    }
            //}


        }
    }
}