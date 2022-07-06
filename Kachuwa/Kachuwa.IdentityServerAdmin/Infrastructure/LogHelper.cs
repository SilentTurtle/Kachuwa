using System;

namespace Kachuwa.IdentityServerAdmin.Infrastructure
{
    public class LogHelper
    {
        public static string GetLogPathFormat(string app)
        {
            var date = $"logs/{DateTimeOffset.Now.ToLocalTime():yyyyMMdd}";
            return $"{date}/{app}.log";
        }
    }
}