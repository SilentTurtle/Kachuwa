using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Kachuwa.Caching
{
    public static class RedisCacheExtensions
    {
        public static T Get<T>(this IDatabase cache, string key)
        {
            return Deserialize<T>(cache.StringGet(key));
        }

        //public static object Get(this IDatabase cache, string key)
        //{
        //    return Deserialize<object>(cache.StringGet(key));
        //}

        public static async Task<T> GetAsync<T>(this IDatabase cache, string key)
        {
            return Deserialize<T>(await cache.StringGetAsync(key));
        }

        //public async static Task<object> GetAsync(this IDatabase cache, string key)
        //{
        //    return Deserialize<object>(await cache.StringGetAsync(key));
        //}

        public static void Set(this IDatabase cache, string key, object value)
        {
            cache.StringSet(key, Serialize(value));
        }
        public static void Set(this IDatabase cache, string key, object value, TimeSpan lifeSpan)
        {
            cache.StringSet(key, Serialize(value), lifeSpan);
        }

        static byte[] Serialize(object o)
        {
            if (o == null)
            {
                return null;
            }
            string json = JsonConvert.SerializeObject(o);
            byte[] serializedResult = System.Text.Encoding.UTF8.GetBytes(json);
            return serializedResult;
            //var bc = new BinaryConverter();
            ////BinaryFormatter binaryFormatter = new BinaryFormatter();
            //using (MemoryStream memoryStream = new MemoryStream())
            //{
            //    binaryFormatter.Serialize(memoryStream, o);
            //    bc.C
            //    byte[] objectDataAsStream = memoryStream.ToArray();
            //    return objectDataAsStream;
            //}
        }

        static T Deserialize<T>(byte[] stream)
        {
            if (stream == null)
            {
                return default(T);
            }

            string json = System.Text.Encoding.UTF8.GetString(stream);
            return JsonConvert.DeserializeObject<T>(json);
            //BinaryFormatter binaryFormatter = new BinaryFormatter();
            //using (MemoryStream memoryStream = new MemoryStream(stream))
            //{
            //    T result = (T)binaryFormatter.Deserialize(memoryStream);
            //    return result;
            //}
        }
    }
}