using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyModel;

namespace Kachuwa.Core.DI
{
    public class KachuwaAppBuilder
    {
        private readonly IApplicationBuilder _app;
        private readonly IServiceProvider _serviceProvider;
        private readonly IHostingEnvironment _hostingEnvironment;

        public KachuwaAppBuilder(IApplicationBuilder app,
            IServiceProvider serviceProvider, IHostingEnvironment hostingEnvironment)
        {
            _app = app;
            _serviceProvider = serviceProvider;
            _hostingEnvironment = hostingEnvironment;
            Configure();
        }

        public Task<IApplicationBuilder> Configure()
        {
            var appBuilderInstances = new List<IAppBuilderRegistrar>();

            var platform = Environment.OSVersion.Platform.ToString();
            var runtimeAssemblyNames = DependencyContext.Default.GetRuntimeAssemblyNames(platform);

            var instances = runtimeAssemblyNames
                .Select(Assembly.Load)
                .SelectMany(a => a.ExportedTypes)
                .Where(t => TypeExtensions.GetInterfaces(t).Contains(typeof(IAppBuilderRegistrar)) && t.GetConstructor(Type.EmptyTypes) != null)
                .Select(y => (IAppBuilderRegistrar)Activator.CreateInstance(y));
            appBuilderInstances.AddRange(instances);

            foreach (var instance in appBuilderInstances)
            {
                //TODO:: check module installed or not
                instance.Configure(_app, _serviceProvider, _hostingEnvironment);
            }

            return Task.FromResult(_app);
        }
    }
}