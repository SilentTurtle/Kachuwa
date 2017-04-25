using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Razor;

namespace Kachuwa.Web.Theme
{
    public class ThemeLocationExpander : IViewLocationExpander
    {


        public void PopulateValues(ViewLocationExpanderContext context)
        {
            var theme = (IThemeConfig)context.ActionContext.HttpContext.Items["Theme"];
            //var value = new Random().Next(0, 1);
            // var theme = value == 0 ? "Theme1" : "Theme2";
            context.Values["themedir"] = "Themes";//theme.Directory;
            context.Values["themename"] = "Default";// theme.FrontendThemeName;
        }
        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            string theme = context.Values["themename"];
            IEnumerable<string> themeLocations = new[]
            {
                $"/Themes/{theme}/Views/{{1}}/{{0}}.cshtml",
                $"/Themes/{theme}/Views/Shared/{{0}}.cshtml"
            };
            viewLocations = themeLocations.Concat(viewLocations);
            return viewLocations;
            //return viewLocations.Select(f => f.Replace("/Views/", "/" + context.Values["themedir"] +"/"+ context.Values["themename"]+ "/Views/"));

        }
    }
}