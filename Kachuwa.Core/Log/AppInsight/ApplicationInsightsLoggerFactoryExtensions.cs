using Microsoft.Extensions.Logging;
using System;
using Kachuwa.Web;
using Microsoft.Extensions.DependencyInjection;

namespace ApplicationInsightsLogging
{
    public static class ApplicationInsightsLoggerFactoryExtensions
    {
        public static ILoggerFactory AddApplicationInsights(
            this ILoggerFactory factory,
            Func<string, LogLevel, bool> filter,
            ApplicationInsightsSettings settings,IServiceProvider serviceProvider)
        {
           var socketApp= serviceProvider.GetService<ApplicationInsightHandler>();
            factory.AddProvider(new ApplicationInsightsLoggerProvider(filter, settings, socketApp));
            return factory;
        }

        public static ILoggerFactory AddApplicationInsights(
            this ILoggerFactory factory,
            ApplicationInsightsSettings settings, IServiceProvider serviceProvider)
        {
            var socketApp = serviceProvider.GetService<ApplicationInsightHandler>();
            factory.AddProvider(new ApplicationInsightsLoggerProvider(null, settings, socketApp));

            return factory;
        }
    }
}
