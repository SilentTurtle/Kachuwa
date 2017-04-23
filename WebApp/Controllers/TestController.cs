using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kachuwa.Web.Razor;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewComponents;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApp.Controllers
{
    public class TestController : Controller
    {
        private readonly IViewRenderService _service;

        public TestController(IViewRenderService service)
        {
            _service = service;
        }
        public async Task<IActionResult> Index()
        {
           var x= ControllerContext;
            string view1 = await _service.RenderToStringAsync("test","index", 1);
            return View();
        }
    }
}
