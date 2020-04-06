using System;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Kachuwa.Web.Notification
{
    public static class NotificationServiceExtensions
    {

        public static IServiceCollection RegisterNotificationService(this IServiceCollection services)
        {
            //default
            services.TryAddSingleton<INotificationBarConfig, NotificationBarConfig>();
            services.AddSingleton<ITempDataProvider, CookieTempDataProvider>();
            services.TryAddTransient<INotificationTempDataWrapper, NotificationTempDataWrapper>();
            services.TryAddSingleton<INotificationService, NotificationService>();
            return services;
        }
        public static IServiceCollection RegisterNotificationService(this IServiceCollection services,
            Action<INotificationBarConfig> config)
        {
            var nConfig = new NotificationBarConfig();
            config(nConfig);
            services.TryAddSingleton<INotificationBarConfig>(nConfig);
            services.AddSingleton<ITempDataProvider, CookieTempDataProvider>();
            services.TryAddSingleton<INotificationTempDataWrapper, NotificationTempDataWrapper>();
            services.TryAddSingleton<INotificationService, NotificationService>();
            return services;
        }
    }
}