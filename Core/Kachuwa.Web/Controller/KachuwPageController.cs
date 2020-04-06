using System.Threading.Tasks;
using Kachuwa.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Options;
using System.Linq;
using Microsoft.AspNetCore.Routing;

namespace Kachuwa.Web
{
    public class KachuwaPageController : BaseController
    {
        public readonly IPageService PageService;
        private readonly IActionDescriptorCollectionProvider actionDescriptorCollectionProvider;

        public KachuwaPageController(IPageService pageService, IActionDescriptorCollectionProvider actionDescriptorCollectionProvider)
        {
            PageService = pageService;
            this.actionDescriptorCollectionProvider = actionDescriptorCollectionProvider;
        }

        [KachuwaPage]
        public async Task<IActionResult> Index(string pageUrl="")
        {
            if (string.IsNullOrEmpty(pageUrl) ||"access-denied"==pageUrl || pageUrl== "page-not-found")
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