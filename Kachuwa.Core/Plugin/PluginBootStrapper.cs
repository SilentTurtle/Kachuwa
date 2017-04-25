using System;
using System.Collections.Generic;
using System.Composition;
using System.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Kachuwa.Log;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace Kachuwa.Plugin
{
    public class PluginBootStrapper
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        private readonly ILogger _logger;
        private readonly IServiceCollection _services;

        [Import]
        public IEnumerable<IPlugin> Plugins { get; set; }

        private CompositionHost Container { get; set; }
        public Dictionary<string, Assembly> PluginDict { get; set; }

        public PluginBootStrapper(IHostingEnvironment hostingEnvironment, ILogger logger,IServiceCollection services)
        {
            _hostingEnvironment = hostingEnvironment;
            _logger = logger;
            _services = services;
            BuildContainer();
        }
        private void BuildContainer()
        {
            try
            {


                var executingAssembly = Assembly.GetEntryAssembly();
                //Path.GetDirectoryName(executableLocation)
                var path = Path.Combine(
                    _hostingEnvironment.ContentRootPath, "Plugins");

                var assemblies = Directory
                    .GetFiles(path, "*.dll", SearchOption.AllDirectories)
                    .Select(AssemblyLoadContext.Default.LoadFromAssemblyPath)
                    .ToList();

                var configuration = new ContainerConfiguration()
                    .WithAssemblies(assemblies).WithAssembly(executingAssembly);

                using (var container = configuration.CreateContainer())
                {
                    Container = container;
                    Plugins = container.GetExports<IPlugin>();
                }

                //   var moduleAssembly = typeof(ViewComponentLibrary.ViewComponents.SimpleViewComponent).GetTypeInfo().Assembly;
                PluginDict = new Dictionary<string, Assembly>();
                foreach (var plugin in Plugins)
                {
                    PluginDict[plugin.Configuration.SystemName] = plugin.Configuration.Assembly;
                    //pluginassemblies.Add(plugin.Configuration.Assembly);
                    //var embeddedFileProvider = new EmbeddedFileProvider(
                    //    plugin.Configuration.Assembly
                    //);
                    //options.FileProviders.Add(embeddedFileProvider);
                }
                var pluginFileProvider = new PluginViewProvider(PluginDict);
                _services.Configure<RazorViewEngineOptions>(options =>
                {
                    options.FileProviders.Add(pluginFileProvider);

                });
            }
            catch (Exception ex)
            {
                _logger.Log(LogType.Error, () => "Plugin Catalog", ex);
            }
        }

      
    }
}