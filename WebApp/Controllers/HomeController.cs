
using Kachuwa.Caching;
using Kachuwa.Log;
using Kachuwa.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace WebApp.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger _logger;
        private readonly IHostingEnvironment _hostingEnvironment;

        public HomeController(IHostingEnvironment hostingEnvironment, ILogger logger)
        {
            _logger = logger;
        }

        [KachuwaCache(Duration = 30)]
        public IActionResult Index()
        {
           
            // return View("Module/ContactUs/Default");
            //return ViewComponent("PluginOne", new {number = 5});
            //return ViewComponent("ContactUsView");
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
           return ViewComponent("HtmlContentManage");
        }

        public IActionResult Error()
        {
            return View();
        }
        public IActionResult Event()
        {
            return View();
        }
    }
}
