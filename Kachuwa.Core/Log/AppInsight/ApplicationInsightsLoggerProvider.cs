using Microsoft.Extensions.Logging;
using System;

namespace ApplicationInsightsLogging
{
    public class ApplicationInsightsLoggerProvider : ILoggerProvider
    {
        private readonly Func<string, LogLevel, bool> _filter;
        private readonly ApplicationInsightsSettings _settings;

        public ApplicationInsightsLoggerProvider(Func<string, LogLevel, bool> filter, ApplicationInsightsSettings settings)
        {
            _filter = filter;
            _settings = settings;
        }

        public ILogger CreateLogger(string name)
        {
            return new ApplicationInsightsLogger(name, _filter, _settings);
        }

        public void Dispose()
        {

        }
    }
}
