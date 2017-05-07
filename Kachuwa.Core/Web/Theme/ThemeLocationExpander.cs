using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;

namespace Kachuwa.Web.Theme
{
    public class ThemeLocationExpander : IViewLocationExpander
    {


        public void PopulateValues(ViewLocationExpanderContext context)
        {
            //TODO:: for multitenat setup
            var theme = (IThemeConfig)context.ActionContext.HttpContext.Items["Theme"];
            //var value = new Random().Next(0, 1);
            // var theme = value == 0 ? "Theme1" : "Theme2";
            context.Values["themedir"] = "Themes";//theme.Directory;

            // var resolver= context.ActionContext.HttpContext.RequestServices.GetService<IThemeResolver>();
            //temporary for single site
            object themeName =null;
            context.ActionContext.RouteData.Values.TryGetValue("Theme", out themeName);
            themeName = themeName == null ? "Default" : themeName;
            context.Values["themename"] = themeName.ToString();// theme.FrontendThemeName;
        }
        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            // we don't want to change layout pages & partials ...
            if (context.IsMainPage)
                return viewLocations;

            var descriptor = (context.ActionContext.ActionDescriptor as ControllerActionDescriptor);
            if (descriptor == null)
            {
                return viewLocations;

            }

            object kpageUrl = context.ActionContext.HttpContext.Items["KPageUrl"];
            string theme = context.Values["themename"];
            //only for layout file look up
            IEnumerable<string> themeLocations = new[]
            {
               // $"/Themes/{theme}/Views/{{1}}/{{0}}.cshtml",
                $"/Themes/{theme}/Views/Shared/{{0}}.cshtml",
                   $"/Themes/Shared/{theme}/Views/Shared/{{0}}.cshtml"
            };
            //default view path must be preserved other wise components wont load
            viewLocations = themeLocations.Concat(viewLocations);
            return viewLocations;

            //return viewLocations.Select(f => f.Replace("/Views/", "/" + context.Values["themedir"] +"/"+ context.Values["themename"]+ "/Views/"));

        }
    }
}