using System;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Kachuwa.Web
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class KachuwaPageAttribute : ActionFilterAttribute
    {
        public KachuwaPageAttribute()
        {

        }

        public string PageUrl { get; private set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var pageUrl = filterContext.RouteData.Values["pageUrl"];

            if (pageUrl != null)
            {
                PageUrl = pageUrl.ToString();
                filterContext.HttpContext.Items.Add("KPageUrl", PageUrl);
            }
            else
            {//landing home page
               
                filterContext.HttpContext.Items.Add("KPageUrl", "landing");
            }

            base.OnActionExecuting(filterContext);
        }
    }
}