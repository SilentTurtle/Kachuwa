using System;
using System.Collections.Generic;
using System.Composition;
using System.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading.Tasks;
using Kachuwa.Core.DI;
using Kachuwa.Web.Razor;
using Kachuwa.Web.Theme;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace WebApp.Controllers
{
    public interface ITest
    {
        int value { get; set; }
        string ping();
    }
    public class Test : ITest
    {
        public int value { get; set; } = 1;

        public string ping()
        {
            return "PONG";
        }
    }
    public class TestServiceRegistrar : IServiceRegistrar
    {
        public void Register(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<ITest, Test>();
        }

        public void Update(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<ITest>(new Test { value = 2 });
            //throw new NotImplementedException();
        }
    }

    public interface IMessageSender
    {
        void Send(string message);
    }

    [Export(typeof(IMessageSender))]
    public class EmailSender : IMessageSender
    {
        public void Send(string message)
        {
            Console.WriteLine(message);
        }
    }
    [Theme("Default")]
    public class HomeController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;


        [Import]
        public IMessageSender MessageSender { get; set; }
        public HomeController(ITest test, IHostingEnvironment hostingEnvironment)
        {
            //_hostingEnvironment = hostingEnvironment;
            //string asdf = test.ping();

            //var assemblies1 = new[] { typeof(Program).GetTypeInfo().Assembly };
            //var configuration1 = new ContainerConfiguration()
            //    .WithAssembly(typeof(Program).GetTypeInfo().Assembly);
            //using (var container = configuration1.CreateContainer())
            //{
            //    var MessageSender = container.GetExport<IMessageSender>();
            //}

            //var executableLocation = Assembly.GetEntryAssembly().Location;
            ////Path.GetDirectoryName(executableLocation)
            //var path = Path.Combine(
            //     _hostingEnvironment.ContentRootPath, "Plugins");

            //var assemblies = Directory
            //            .GetFiles(path, "*.dll", SearchOption.AllDirectories)
            //            .Select(AssemblyLoadContext.Default.LoadFromAssemblyPath)
            //            .ToList();
            //var configuration = new ContainerConfiguration()
            //    .WithAssemblies(assemblies);

            //using (var container = configuration.CreateContainer())
            //{
            //    MessageSenders = container.GetExports<IMessageSender>();
            //}
        }

        public IEnumerable<IMessageSender> MessageSenders { get; set; }

        public IActionResult Index()
        {

            //return ViewComponent("PluginOne", new {number = 5});
            return View();
            //Plugin/PluginOne/Home/index
            //return View("Plugin/PluginOne/pluginone");
        }

        public IActionResult About()
        {
            return View("Plugin/PluginOne/pluginone");
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
