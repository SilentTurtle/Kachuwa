using Microsoft.Extensions.Logging;
using System;
using Kachuwa.Web;

namespace ApplicationInsightsLogging
{
    public class ApplicationInsightsLoggerProvider : ILoggerProvider
    {
        private readonly Func<string, LogLevel, bool> _filter;
        private readonly ApplicationInsightsSettings _settings;
        private readonly ApplicationInsightHandler _applicationInsightHandler;

        public ApplicationInsightsLoggerProvider(Func<string, LogLevel, bool> filter, ApplicationInsightsSettings settings, ApplicationInsightHandler applicationInsightHandler)
        {
            _filter = filter;
            _settings = settings;
            _applicationInsightHandler = applicationInsightHandler;
        }

        public ILogger CreateLogger(string name)
        {
            return new ApplicationInsightsLogger(name, _filter, _settings, _applicationInsightHandler);
        }

        public void Dispose()
        {

        }
    }
}
