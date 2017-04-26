using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Kachuwa.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace Kachuwa.Core.DI
{
    public class Bootstrapper : IBootstrapper
    {
        private readonly IServiceCollection _serviceCollection;
        private readonly IConfigurationRoot _configuration;
        public Bootstrapper(IServiceCollection serviceCollection, IConfigurationRoot configuration)
        {
            _serviceCollection = serviceCollection;
            _configuration = configuration;
            Init();
        }
        public void Init()
        {
            Build();
        }

        public bool Build()
        {
            FindThenBuild();
            return true;
        }
        private void FindThenBuild()
        {
            //will return assembly start with kachuwa
            var assesmblies = AppDomain.CurrentDomain.GetAssemblies();
            var serviceInstances = new List<IServiceRegistrar>();
            foreach (var assembly in assesmblies)
            {
                var instances = from t in assembly.GetTypes()
                                where TypeExtensions.GetInterfaces(t).Contains(typeof(IServiceRegistrar))
                                      && t.GetConstructor(Type.EmptyTypes) != null
                                select Activator.CreateInstance(t) as IServiceRegistrar;

                serviceInstances.AddRange(instances);
            }
            foreach (var instance in serviceInstances)
            {
                instance.Register(_serviceCollection, _configuration);
            }
            //var assemblies = AppDomain.CurrentDomain.GetAssemblies()
            //    .Where(a => a.GetTypes().Contains(typeof(ApiController)))
            //    .ToArray();
            //// Register your Web API controllers.

            //Builder.RegisterApiControllers(assemblies);
            // AutofacPlugin.Register(Builder);
            // Container = (Container)Builder.Build();

            //TODO build
            //_serviceCollection.BuildServiceProvider();

            foreach (var instance in serviceInstances)
            {
                instance.Update(_serviceCollection);
            }

            // DependencyResolver.SetResolver(new AutofacDependencyResolver(Container));
            // GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(Container);

        }


    }
}