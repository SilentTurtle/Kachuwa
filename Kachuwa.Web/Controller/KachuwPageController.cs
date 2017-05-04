using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Kachuwa.Web
{
    public class KachuwPageController : BaseController
    {
        public IPageService PageService { get; }

        public KachuwPageController(IPageService pageService)
        {
            PageService = pageService;
        }

        [KachuwaPage]
        public async Task<IActionResult> Index(string pageUrl)
        {
            if (await PageService.CheckPageExist(pageUrl))
            {
                return View();
            }
            return PartialView("_PageNotFound");
        }
    }
}