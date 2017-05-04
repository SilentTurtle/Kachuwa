using Microsoft.Extensions.Logging;
using System;
using Kachuwa.Web;

namespace Kachuwa.Log.Insight
{
    public class ApplicationInsightsLoggerProvider : Microsoft.Extensions.Logging.ILoggerProvider
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

        public Microsoft.Extensions.Logging.ILogger CreateLogger(string name)
        {
            return new ApplicationInsightsLogger(name, _filter, _settings, _applicationInsightHandler);
        }

        public void Dispose()
        {

        }
    }
}
