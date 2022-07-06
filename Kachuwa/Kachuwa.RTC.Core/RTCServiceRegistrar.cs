using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Kachuwa.Core.DI;
using Kachuwa.RTC.Hubs;
using Kachuwa.Web.Notification;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;

namespace Kachuwa.RTC
{
    public class RTCServiceRegistrar : IServiceRegistrar
    {
        public void Update(IServiceCollection serviceCollection)
        {
            
        }

        public void Register(IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.AddSingleton<IRTCConnectionManager, MemoryConnectionManager>();
            //serviceCollection.AddSingleton<IRTCConnectionManager, RTCPersistentConnectionManager>();
            serviceCollection.AddSingleton<IRTCUserService, RTCPersistentConnectionManager>();
            //serviceCollection.AddSingleton<ITagHelperComponent, RTCNotificationTagHelper>();
           // serviceCollection.AddSingleton<ITagHelperComponent, SignalRTagHelper>();
           // serviceCollection.AddSingleton<INotificationService, RTCNotificationService>();
            serviceCollection.AddSingleton<IChatService, ChatService>();
            
            serviceCollection.AddSingleton<List<UserCall>>();
            serviceCollection.AddSingleton<List<CallOffer>>();

        }
    }

    public interface IRTCRouter 
    {
        Type Hub { get; set; }
        string Path { get; set; }
    }
    public class SignalRRouter: IRTCRouter
    {
        public Type Hub { get; set; } = typeof(KachuwaUserHub);
        public string Path { get; set; } = "";
    }
    //public class RTCAppBuilder : IAppBuilderRegistrar {
    //    public void Configure(IApplicationBuilder app, IServiceProvider serviceProvider, IHostingEnvironment env)
    //    {
    //        app.UseSignalR(routes =>  // <-- SignalR
    //        {
            
    //            routes.MapHub<KachuwaUserHub>("/hubs/user");
    //            routes.MapHub<CartHub>("/hubs/cart");
    //            routes.RegisterAll();
    //        });
    //    }
    //}

    //public static class RTCExtensions
    //{
    //    public static void RegisterAll(this HubRouteBuilder routes)
    //    {
    //        var appBuilderInstances = new List<IRTCRouter>();

    //        var platform = Environment.OSVersion.Platform.ToString();
    //        var runtimeAssemblyNames = DependencyContext.Default.GetRuntimeAssemblyNames(platform);

    //        var instances = runtimeAssemblyNames
    //            .Select(Assembly.Load)
    //            .SelectMany(a => a.ExportedTypes)
    //            .Where(t => TypeExtensions.GetInterfaces(t).Contains(typeof(IRTCRouter)) && t.GetConstructor(Type.EmptyTypes) != null)
    //            .Select(y => (IRTCRouter)Activator.CreateInstance(y));
    //        appBuilderInstances.AddRange(instances);
          
    //    }
    //}
}