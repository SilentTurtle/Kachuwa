using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Kachuwa.Core.DI;
using Kachuwa.Log;
using Kachuwa.Reflection;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;

namespace Kachuwa.Web.Module
{
    class Class1
    {
    }

    public class ModuleRegistrar
    {
        private readonly IServiceCollection _services;
        private readonly ILogger _logger;
        public ConcurrentDictionary<string, Assembly> ModulesAssemblies { get; set; }

        public ModuleRegistrar(IServiceCollection services,ILogger logger)
        {
            _services = services;
            _logger = logger;
            ModulesAssemblies=new ConcurrentDictionary<string, Assembly>();
            Register();
        }
        public bool Register()
        {
            try
            {

           
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
            _logger.Log(LogType.Trace, () => $"Total {modules.Count} Modules Found.");
            foreach (var instance in modules)
            {
                ModulesAssemblies.TryAdd(instance.Name, instance.Assembly);
            }
            var modulesFileProvider = new ModuleViewProvider(ModulesAssemblies);
            _services.Configure<RazorViewEngineOptions>(options =>
            {
                options.FileProviders.Add(modulesFileProvider);

            });
            _logger.Log(LogType.Trace, () => $"Razor engine module file provider added.");
            return true;
            }
            catch (Exception e)
            {
                _logger.Log(LogType.Error, () => $"Razor engine module adding error.",e);
                throw;
            }
        }
    }
}
