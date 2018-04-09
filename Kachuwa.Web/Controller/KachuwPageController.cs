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

        public async Task<IActionResult> PageNotFound()
        {
            return PartialView("_PageNotFound");
        }

        [HttpGet("page/dev/routes")]
        public IActionResult GetRoutes()
        {
            //var routes = actionDescriptorCollectionProvider.ActionDescriptors.Items.Select(x => new {
            //    Action = x.RouteValues["Action"],
            //    Controller = x.RouteValues["Controller"],
            //    Name = x.AttributeRouteInfo.Name,
            //    Template = x.AttributeRouteInfo.Template
            //}).ToList();
            //var routes = RouteData.Routers.OfType<RouteCollection>().ToList();
            //return Ok(routes);
            var routes = actionDescriptorCollectionProvider.ActionDescriptors.Items.Select(x => new {
                Action = x.RouteValues["Action"],
                Controller = x.RouteValues["Controller"],
                Name = x.AttributeRouteInfo.Name,
                Template = x.AttributeRouteInfo.Template
            }).ToList();
            return Ok(routes);
        }
    }
}