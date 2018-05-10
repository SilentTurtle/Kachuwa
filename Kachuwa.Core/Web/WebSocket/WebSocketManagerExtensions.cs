using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Kachuwa.Core.DI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Kachuwa.Web
{
    public static class WebSocketManagerExtensions
    {
        public static IServiceCollection AddWebSocketManager(this IServiceCollection services)
        {
            services.AddTransient<WebSocketConnectionManager>();
            //services.AddTransient<WebSocketHandler, NotificationHandler>();
            //services.AddTransient<WebSocketHandler, ChatHandler>();
            var coreAssembly = typeof(WebSocketHandler).GetTypeInfo().Assembly;
            foreach (var type in coreAssembly.ExportedTypes)
            {
                if (type.GetTypeInfo().BaseType == typeof(WebSocketHandler))
                {
                    services.AddSingleton(type);
                }
            }
            //var assesmblies = AppDomain.CurrentDomain.GetAssemblies();

            //foreach (var assembly in assesmblies)
            //{
            //    var types = assembly.GetExportedTypes();
            //    foreach (var type in types)
            //    {
            //        if (type.GetTypeInfo().BaseType == typeof(WebSocketHandler))
            //        {
            //            services.AddSingleton(type);
            //        }
            //    }


            //}

            return services;
        }

        public static IApplicationBuilder MapWebSocketManager(this IApplicationBuilder app,
                                                              PathString path,
                                                              WebSocketHandler handler)
        {
            app.Map(path, (x) =>
            {
                x.UseMiddleware<WebSocketManagerMiddleware>(handler);
            });
            return app;
            // return app.Map(path, (_app) => _app.UseMiddleware<WebSocketManagerMiddleware>(handler));

        }
        public static IApplicationBuilder UseKSockets(this IApplicationBuilder app,
                                                             IServiceProvider serviceProvider)
        {
            app.MapWebSocketManager(SocketHubs.NotificationHub, serviceProvider.GetService<NotificationHandler>());
            app.MapWebSocketManager(SocketHubs.ChatHub, serviceProvider.GetService<ChatHandler>());
            app.MapWebSocketManager(SocketHubs.AppInsight, serviceProvider.GetService<ApplicationInsightHandler>());

            return app;

        }
    }
}
