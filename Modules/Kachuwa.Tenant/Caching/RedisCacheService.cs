using System;
using System.Threading.Tasks;
using Kachuwa.Caching;
using Kachuwa.Web;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Kachuwa.Tenant.Caching
{
    public class RedisCacheService : ICacheService
    {
        private readonly Lazy<ConnectionMultiplexer> _lazyConnection;

        private readonly object _lock = new Object();
        private const int ExpireInSeconds = 600;//10 min
        public ConnectionMultiplexer Connection => _lazyConnection.Value;

        public RedisCacheService(IOptions<RedisConfiguration> optionConfig)
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
        private Tenant GetTenant()
        {
            var currentTenant = (CurrentTenant)ContextResolver.Context.Items[TenantConstant.TenantContextKey];
            return currentTenant.Info;
        }
        public void Dispose()
        {
            Connection.Dispose();
        }
        public T Get<T>(string key)
        {
            key = GetTenant().Name + "_" + key;
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
            key = GetTenant().Name + "_" + key;
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
            key = GetTenant().Name + "_" + key;
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
            key = GetTenant().Name + "_" + key;
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
            key = GetTenant().Name + "_" + key;
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
            key = GetTenant().Name + "_" + key;
            IDatabase cache = Connection.GetDatabase();
            cache.KeyDelete(key);
        }

        public void Flush()
        {

        }


    }
}