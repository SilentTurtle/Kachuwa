using System;
using System.Text;
using Newtonsoft.Json;

namespace Kachuwa.Configuration
{
    public class KachuwaAppConfig
    {
        public string AppName { get; set; }
        public string SiteUrl { get; set; }
        public bool IsInstalled { get; set; }
        public string DbProvider { get; set; }
        public string Framework { get; set; }
        public string Version { get; set; }
        public bool EnableLogging { get; set; }
        public bool EnableDbLogging { get; set; }
        public string Theme { get; set; }
        public string AdminTheme { get; set; }
        public string FacebookAppId { get; set; }
        public bool RequireConfirmedEmail { get; set; }
        public int PasswordLength { get; set; }
        public bool RequireNonAlphanumeric { get; set; }
        public bool RequireUppercase { get; set; }
        public SMTMConfig SMTMConfig { get; set; }
        public string JobConnection { get; set; }
        //public string BaseCulture { get; set; }
    }
    public class SMTMConfig
    {
        public string Host { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool UseSSL { get; set; }
        public int Port { get; set; }

    }

}
