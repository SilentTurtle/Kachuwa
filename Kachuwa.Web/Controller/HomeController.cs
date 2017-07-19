
using System.Threading.Tasks;
using Kachuwa.Caching;
using Kachuwa.Log;
using Kachuwa.Web;
using Kachuwa.Web.Module;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace Kachuwa.Web
{
    public class HomeController : BaseController
    {
        private readonly ILogger _logger;
        private readonly IModuleManager _moduleManager;
        private readonly IHostingEnvironment _hostingEnvironment;

        public HomeController(IHostingEnvironment hostingEnvironment, ILogger logger,IModuleManager moduleManager)
        {
            _logger = logger;
            _moduleManager = moduleManager;
          
        }
        public async Task<IActionResult> Index()
        {
            //var blog = await _moduleManager.FindAsync("Blog");
            //if (await _moduleManager.InstallAsync(blog))
            //    await _moduleManager.UpdateModule(blog);

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
