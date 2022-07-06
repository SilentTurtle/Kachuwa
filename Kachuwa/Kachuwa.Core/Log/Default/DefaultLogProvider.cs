using System.Diagnostics;
using Microsoft.AspNetCore.Hosting;

namespace Kachuwa.Log
{
    /// <summary>
    /// Default Implementation of ILogProvider
    /// </summary>
    public class DefaultLogProvider : ILogProvider
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ILoggerSetting _loggerSetting;

        public DefaultLogProvider(IWebHostEnvironment hostingEnvironment, ILoggerSetting loggerSetting)
        {
            _hostingEnvironment = hostingEnvironment;
            _loggerSetting = loggerSetting;
        }
        public ILogger GetLogger(string name)
        {
            return new FileBaseLogger(_hostingEnvironment, _loggerSetting);
        }
       
    }
}