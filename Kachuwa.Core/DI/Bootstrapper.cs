using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Kachuwa.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Kachuwa.Core.DI
{
    public class Bootstrapper
    {
        private readonly IServiceCollection _serviceCollection;
        public Bootstrapper(IServiceCollection serviceCollection )
        {
            _serviceCollection = serviceCollection;
            Build();
        }
        private  void Build()
        {

            var assesmblies = AppDomain.CurrentDomain.GetAssemblies();
            //To avoid this issue use the GetReferencedAssemblies() method on 
            //http://docs.autofac.org/en/latest/register/scanning.html
            // var assesmblies = BuildManager.GetReferencedAssemblies().Cast<Assembly>();
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
                instance.Register(_serviceCollection);
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