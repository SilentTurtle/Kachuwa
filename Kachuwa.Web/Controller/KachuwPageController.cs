using System.Threading.Tasks;
using Kachuwa.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Options;
using System.Linq;
using Microsoft.AspNetCore.Routing;

namespace Kachuwa.Web
{
    public class KachuwPageController : BaseController
    {
        public readonly IPageService PageService;
        private readonly IActionDescriptorCollectionProvider actionDescriptorCollectionProvider;

        public KachuwPageController(IPageService pageService, IActionDescriptorCollectionProvider actionDescriptorCollectionProvider)
        {
            PageService = pageService;
            this.actionDescriptorCollectionProvider = actionDescriptorCollectionProvider;
        }

        [KachuwaPage]
        public async Task<IActionResult> Index(string pageUrl="")
        {
            if (string.IsNullOrEmpty(pageUrl))
            {
                return View();
            }
            else if (await PageService.CheckPageExist(pageUrl))
            {
                return View();
            }
            return Redirect("/page-not-found");
        }
      
    }
}