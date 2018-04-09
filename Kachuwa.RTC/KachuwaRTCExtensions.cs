using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Sockets;

namespace Kachuwa.RTC
{
    public static class KachuwaRTCExtensions
    {
        public static IApplicationBuilder UseKachuwaRTC(this IApplicationBuilder app, Action<HubRouteBuilder> configure)
        {
            app.UseSockets((Action<SocketRouteBuilder>)(routes => configure(new HubRouteBuilder(routes))));
            return app;
        }
    }
}