using System.Threading.Tasks;
using Kachuwa.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Kachuwa.Web
{
    public class KachuwPageController : BaseController
    {
        public readonly IPageService PageService;

        public KachuwPageController(IPageService pageService)
        {
            PageService = pageService;
        }

        [KachuwaPage]
        public async Task<IActionResult> Index(string pageUrl="")
        {
            
            //home landing page
            if (string.IsNullOrEmpty(pageUrl))
            {
                return View();
            }
            else if (await PageService.CheckPageExist(pageUrl))
            {
                return View();
            }
            return PartialView("_PageNotFound");
        }
    }
}