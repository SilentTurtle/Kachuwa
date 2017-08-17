using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Kachuwa.Log;
using Kachuwa.Reflection;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Kachuwa.Web.Module
{
    public class ModuleRegistrar
    {
        private readonly IServiceCollection _services;
        private readonly ILogger _logger;

        public ModuleRegistrar(IServiceCollection services, ILogger logger)
        {
            _services = services;
            _logger = logger;
            Register();
        }
        public bool Register()
        {
            try
            {
                var serviceProvider = _services.BuildServiceProvider();
                var moduleService = serviceProvider.GetService<IModuleService>();

                var assesmblies = AppDomain.CurrentDomain.GetAssemblies();
                var modules = new List<IModule>();
                foreach (var assembly in assesmblies)
                {
                    var instances = from t in assembly.GetTypes()
                                    where TypeExtensions.GetInterfaces(t).Contains(typeof(IModule))
                                          && t.GetConstructor(Type.EmptyTypes) != null
                                    select Activator.CreateInstance(t) as IModule;

                   
                    modules.AddRange(instances);
                }
                var installedModules = moduleService.Service.GetList("Where IsInstalled=1",new{});
                if (installedModules.Any())
                {
                    foreach (var installed in installedModules)
                    {
                        modules.Find(e => e.Name.ToLower() == installed.Name.ToLower()).IsInstalled = true;
                    }
                }
                _services.TryAddSingleton(new ModuleContainer(modules));
                _logger.Log(LogType.Trace, () => $"Total {modules.Count} Modules Found.");
                serviceProvider = _services.BuildServiceProvider();
                var moduleManager = serviceProvider.GetService<IModuleManager>();
                var modulesFileProvider = new ModuleViewProvider(moduleManager);

                _services.Configure<RazorViewEngineOptions>(options =>
                {
                    options.FileProviders.Add(modulesFileProvider);
                });
                _logger.Log(LogType.Trace, () => $"Razor engine module file provider added.");
                return true;
            }
            catch (Exception e)
            {
                _logger.Log(LogType.Error, () => $"Razor engine module adding error.", e);
                throw;
            }
        }
    }
}