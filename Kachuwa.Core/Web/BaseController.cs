using System;
using System.Text;
using Kachuwa.Configuration;
using Kachuwa.Log;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using Microsoft.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Kachuwa.Web
{
    [LogError]
    public class BaseController : Controller
    {
        public readonly ModelService ModelService = new ModelService();
        public readonly KachuwaAppConfig KachuwaAppConfig;
        public BaseController()
        {
            var kachuwaconfig = ContextResolver.Context.RequestServices.GetService<IOptions<KachuwaAppConfig>>();
            KachuwaAppConfig = kachuwaconfig.Value;


        }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            if (!KachuwaAppConfig.IsInstalled)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    controller = "Installer",
                    action = "Index"
                }));
            }
        }
        public RedirectResult RedirectToAnother(string url)
        {
            return base.Redirect(url);
        }
        // protected override void OnActionExecuting(ActionExecutingContext filterContext)
        //{
        //if (!HttpContext.User.Identity.IsAuthenticated)
        //{
        //    if (HttpContext.Request.Cookies.Get(ApplicationClaim.Anonymous) == null)
        //    {
        //        var userId = Guid.NewGuid().ToString();
        //        HttpCookie cookie = new HttpCookie(ApplicationClaim.Anonymous);
        //        cookie.HttpOnly = true;
        //        cookie.Expires = DateTime.Now.AddMinutes(30);
        //        cookie.Value = userId;
        //        HttpContext.Response.Cookies.Add(cookie);
        //    }
        //}
        //base.OnActionExecuting(filterContext);
        // }
    }
}
