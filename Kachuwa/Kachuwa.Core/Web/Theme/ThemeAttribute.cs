using System;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Kachuwa.Web.Theme
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ThemeAttribute : ActionFilterAttribute
    {
        public ThemeAttribute(string theme)
        {
            Theme = theme;
        }

        public string Theme { get; private set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            filterContext.RouteData.Values.Add("Theme", Theme);
            base.OnActionExecuting(filterContext);
        }
    }
}