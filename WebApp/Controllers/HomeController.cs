using System;
using System.Collections.Generic;
using System.Composition;
using System.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading.Tasks;
using Kachuwa.Caching;
using Kachuwa.Core.DI;
using Kachuwa.Log;
using Kachuwa.Web;
using Kachuwa.Web.Razor;
using Kachuwa.Web.Theme;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace WebApp.Controllers
{
    //public interface ITest
    //{
    //    int value { get; set; }
    //    string ping();
    //}
    //public class Test : ITest
    //{
    //    public int value { get; set; } = 1;

    //    public string ping()
    //    {
    //        return "PONG";
    //    }
    //}
    //public class TestServiceRegistrar : IServiceRegistrar
    //{
    //    public void Register(IServiceCollection serviceCollection, IConfigurationRoot configuration)
    //    {
    //        serviceCollection.AddSingleton<ITest, Test>();
    //    }

    //    public void Update(IServiceCollection serviceCollection)
    //    {
    //        serviceCollection.AddSingleton<ITest>(new Test { value = 2 });
    //        //throw new NotImplementedException();
    //    }
    //}

    //public interface IMessageSender
    //{
    //    void Send(string message);
    //}

    //[Export(typeof(IMessageSender))]
    //public class EmailSender : IMessageSender
    //{
    //    public void Send(string message)
    //    {
    //        Console.WriteLine(message);
    //    }
    //}
    // [Theme("Default")]
    
    public class HomeController : BaseController
    {
        private readonly ILogger _logger;
        private readonly IHostingEnvironment _hostingEnvironment;

        public HomeController(IHostingEnvironment hostingEnvironment, ILogger logger)
        {
            _logger = logger;
            _logger.Log(LogType.Info, () => "HomePage constructor Initialized");
        }

        [KachuwaCache(Duration = 15)]
        public IActionResult Index()
        {
            _logger.Log(LogType.Info, () => "HomePage index Initialized");

            //return ViewComponent("PluginOne", new {number = 5});
            return View();
            //Plugin/PluginOne/Home/index
            //return View("Plugin/PluginOne/pluginone");
        }

        public IActionResult About()
        {
            _logger.Log(LogType.Info, () => "HomePage plugin base Initialized");
            return View("Plugin/PluginOne/pluginone");
        }

        public IActionResult Contact()
        {
            throw new Exception("fuck me");
            ViewData["Message"] = "Your contact page.";
            _logger.Log(LogType.Info, () => "HomePage contactpage Initialized");
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
