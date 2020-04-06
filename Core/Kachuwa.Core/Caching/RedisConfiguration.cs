using System.Collections.Generic;

namespace Kachuwa.Caching
{
    public class RedisConfiguration
    {
        public List<string> EndPoints { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool UseSsl { get; set; }
        public int TimeOut { get; set; }
        public int SyncTimeOut { get; set; }

    }
}