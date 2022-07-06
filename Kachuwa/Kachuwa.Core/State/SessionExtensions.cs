using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Kachuwa.State
{
    public static class SessionExtensions
    {
        public static void Set(this ISession session, string key, object value)
        {
            string json = JsonConvert.SerializeObject(value);
            byte[] serializedResult = System.Text.Encoding.UTF8.GetBytes(json);
            session.Set(key, serializedResult);
        }

        public static T Get<T>(this ISession session, string key)
        {
            var value = session.Get(key);
            if (value == null)
                return default(T);

            string json = System.Text.Encoding.UTF8.GetString(value);

            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(json);
        }

        public static bool IsExists(this ISession session, string key)
        {
            return session.Get(key) != null;
        }
    }
}