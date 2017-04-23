using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Converters;
using StackExchange.Redis;

namespace Kachuwa.Caching
{
    public class RedisCache : ICache
    {
        private readonly Lazy<ConnectionMultiplexer> _lazyConnection;

        private readonly object _lock = new Object();
        private const int ExpireInSeconds = 600;//10 min
        public ConnectionMultiplexer Connection => _lazyConnection.Value;

        public RedisCache(IOptions<RedisConfiguration> optionConfig)
        {
            if (optionConfig == null)
                throw new Exception("");
            var config = optionConfig.Value;

            _lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
            {
                ConfigurationOptions options = new ConfigurationOptions();
                foreach (var endPoint in config.EndPoints)
                {
                    options.EndPoints.Add(endPoint);
                }

                options.Ssl = config.UseSsl;
                options.Password = config.Password;
                options.ConnectTimeout = config.TimeOut;
                options.SyncTimeout = config.SyncTimeOut;
                return ConnectionMultiplexer.Connect(options);
            });

        }

        public void Dispose()
        {
            Connection.Dispose();
        }
        public T Get<T>(string key)
        {
            IDatabase cache = Connection.GetDatabase();
            lock (_lock)
            {
                var obj = cache.Get<T>(key);
                return obj;
            }

        }

        public T Get<T>(string key, Func<T> dataFactory)
        {
            return Get<T>(key, TimeSpan.FromMinutes(ExpireInSeconds), dataFactory);
        }

        public T Get<T>(string key, TimeSpan cachingLife, Func<T> dataFactory)
        {
            IDatabase cache = Connection.GetDatabase();
            lock (_lock)
            {
                var obj = cache.Get<T>(key);
                if (obj == null)
                {
                    obj = dataFactory();
                    if (obj != null)
                    {
                        cache.Set(key, obj, cachingLife);
                    }
                }
                return obj;
            }
        }

        public T Get<T>(string key, int expireInSeconds, Func<T> dataFactory)
        {
            IDatabase cache = Connection.GetDatabase();
            lock (_lock)
            {
                var obj = cache.Get<T>(key);
                if (obj == null)
                {
                    obj = dataFactory();
                    if (obj != null)
                    {
                        cache.Set(key, obj, TimeSpan.FromSeconds(expireInSeconds));
                    }
                }
                return obj;
            }
        }

        public async Task<T> GetAsync<T>(string key, Func<Task<T>> dataFactory)
        {
            return await GetAsync<T>(key, ExpireInSeconds, dataFactory);
        }

        public async Task<T> GetAsync<T>(string key, int expireInSeconds, Func<Task<T>> dataFactory)
        {
            IDatabase cache = Connection.GetDatabase();
            var obj = await cache.GetAsync<T>(key);
            if (obj == null)
            {
                obj = await dataFactory();
                if (obj != null)
                {
                    cache.Set(key, obj, TimeSpan.FromSeconds(expireInSeconds));
                }
            }
            return obj;
        }

        public async Task<T> GetAsync<T>(string key, Func<Task<T>> dataFactory, TimeSpan cachingLife)
        {
            IDatabase cache = Connection.GetDatabase();
            var obj = await cache.GetAsync<T>(key);
            if (obj == null)
            {
                obj = await dataFactory();
                if (obj != null)
                {
                    cache.Set(key, obj, cachingLife);
                }
            }
            return obj;
        }

        public void Remove(string key)
        {
            IDatabase cache = Connection.GetDatabase();
            cache.KeyDelete(key);
        }

        public void Flush()
        {
           
        }

      
    }
}