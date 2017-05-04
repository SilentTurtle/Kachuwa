using Microsoft.Extensions.Logging;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Kachuwa.Web;


namespace Kachuwa.Log.Insight
{

 
    public class ApplicationInsightsLogger :Microsoft.Extensions.Logging.ILogger
    {
        private readonly ApplicationInsightHandler _applicationInsightHandler;
        private readonly string _name;
        private readonly Func<string, LogLevel, bool> _filter;
        private readonly ApplicationInsightsSettings _settings;

        public ApplicationInsightsLogger(string name, ApplicationInsightHandler applicationInsightHandler)
            : this(name, null, new ApplicationInsightsSettings(), applicationInsightHandler)
        {
            _applicationInsightHandler = applicationInsightHandler;
        }

        public ApplicationInsightsLogger(string name, Func<string, LogLevel, bool> filter, ApplicationInsightsSettings settings, ApplicationInsightHandler applicationInsightHandler)
        {
            _applicationInsightHandler= applicationInsightHandler;
            _name = string.IsNullOrEmpty(name) ? nameof(ApplicationInsightsLogger) : name;
            _filter = filter;
            _settings = settings;
           //// _telemetryClient = new TelemetryClient();

           // if (_settings.DeveloperMode.HasValue)
           // {
           //     //TelemetryConfiguration.Active.TelemetryChannel.DeveloperMode = _settings.DeveloperMode;
           // }

           // if(!_settings.DeveloperMode.Value)
           // {
           //     if (string.IsNullOrWhiteSpace(_settings.InstrumentationKey))
           //     {
           //         throw new ArgumentNullException(nameof(_settings.InstrumentationKey));
           //     }

           //    // TelemetryConfiguration.Active.InstrumentationKey = _settings.InstrumentationKey;
           //    // _telemetryClient.InstrumentationKey = _settings.InstrumentationKey;
           // }
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return NoopDisposable.Instance;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return (_filter == null || _filter(_name, logLevel));
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            if (exception != null)
            {
               // _telemetryClient.TrackException(new ExceptionTelemetry(exception));
                Task.Run(async () =>
                {
                    await _applicationInsightHandler.SendMessageToAllAsync(new Message()
                    {
                        Data = exception.Message.ToString(),
                        MessageType = MessageType.Text
                    });
                });
                return;
            }

            var message = string.Empty;
            if (formatter != null)
            {
                message = formatter(state, exception);
            }
            else
            {
                if (state != null)
                {
                    message += state;
                }
            }
            if (!string.IsNullOrEmpty(message))
            {
                //TODO:: on/off setting
                Task.Run(async () =>
                {
                    await _applicationInsightHandler.SendMessageToAllAsync(new Message()
                    {
                        Data = message,
                        MessageType = MessageType.Text
                    });
                });
                
                
               // _telemetryClient.TrackTrace(message, GetSeverityLevel(logLevel));
            }
        }


        //private static SeverityLevel GetSeverityLevel(LogLevel logLevel)
        //{
        //    switch (logLevel)
        //    {
        //        case LogLevel.Critical: return SeverityLevel.Critical;
        //        case LogLevel.Error: return SeverityLevel.Error;
        //        case LogLevel.Warning: return SeverityLevel.Warning;
        //        case LogLevel.Information: return SeverityLevel.Information;
        //        case LogLevel.Trace:
        //        default: return SeverityLevel.Verbose;
        //    }
        //}

        private class NoopDisposable : IDisposable
        {
            public static NoopDisposable Instance = new NoopDisposable();

            public void Dispose()
            {
            }
        }
    }
}
