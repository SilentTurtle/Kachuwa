//using Kachuwa.Core.DI;
//using System;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Configuration;
//using Microsoft.AspNetCore.Mvc.Razor;
//using Microsoft.Extensions.FileProviders;
//using System.Reflection;

//namespace Kachuwa.Web
//{
//    public class KachuwaWebServiceRegistrar : IServiceRegistrar
//    {
//        private bool _isInstalled = false;
//        public void Register(IServiceCollection serviceCollection, IConfiguration configuration)
//        {

//            var strIsInstalled = configuration["KachuwaAppConfig:IsInstalled"].ToString().ToLower();
//            _isInstalled = strIsInstalled != "false";

//            serviceCollection.RegisterKachuwaWebServices(_isInstalled);
//            var embeddedAssembly = new EmbeddedFileProvider(typeof(KachuwaWebServiceRegistrar).GetTypeInfo().Assembly);
//            serviceCollection.Configure<RazorViewEngineOptions>(opts =>
//            {
//                opts.FileProviders.Add(embeddedAssembly);
//            });

//        }

//        public void Update(IServiceCollection serviceCollection)
//        {
//            if (_isInstalled)
//            {
//                // var builder = serviceCollection.BuildServiceProvider();
//                //  var settingService = builder.GetService<ISettingService>();
//                // serviceCollection.AddSingleton(settingService.CrudService.Get(1));
//            }
//        }
//    }
//}
