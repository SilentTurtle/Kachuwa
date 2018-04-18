using Kachuwa.Core.DI;
using Kachuwa.Web.Notification;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Kachuwa.RTC
{
    public class RTCServiceRegistrar : IServiceRegistrar
    {
        public void Update(IServiceCollection serviceCollection)
        {
            
        }

        public void Register(IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.AddSingleton<IRTCConnectionManager,RTCConnectionManager>();
            serviceCollection.AddSingleton<ITagHelperComponent, RTCNotificationTagHelper>();
            //serviceCollection.AddSingleton<INotificationService, RTCNotificationService>();
        }
    }
}