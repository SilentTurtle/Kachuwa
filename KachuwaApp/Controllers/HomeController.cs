using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Kachuwa.Core.DI;
using Microsoft.Extensions.DependencyInjection;

namespace KachuwaApp.Controllers
{
    public interface ITest
    {
        int value { get; set;}
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
    public class HomeController : Controller
    {
        public HomeController(ITest test)
        {
           string asdf= test.ping();
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
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
