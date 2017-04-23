using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Kachuwa.Caching
{
    public interface ICache : IDisposable
    {
        T Get<T>(string key);
        T Get<T>(string key, Func<T> dataFactory);
        T Get<T>(string key, TimeSpan cachingLife, Func<T> dataFactory);
        T Get<T>(string key, int expireInSeconds, Func<T> dataFactory);
        Task<T> GetAsync<T>(string key, Func<Task<T>> dataFactory);
        Task<T> GetAsync<T>(string key, int expireInSeconds, Func<Task<T>> dataFactory);
        Task<T> GetAsync<T>(string key, Func<Task<T>> dataFactory, TimeSpan cachingLife);
        void Remove(string key);
        void Flush();
    }
}
