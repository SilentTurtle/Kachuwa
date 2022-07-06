using System;
using Kachuwa.Core.DI;
using Kachuwa.Web.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace Kachuwa.Web
{
    public class WebApplicationBuilderRegistrar : IAppBuilderRegistrar
    {
        public void Configure(IApplicationBuilder app, IServiceProvider serviceProvider, IWebHostEnvironment env)
        {
            app.UseMiddleware<CustomHeaderMiddleware>();
            app.UseMiddleware<ImageResizerMiddleware>();
            app.UseStaticHttpContext();
        }
    }
}