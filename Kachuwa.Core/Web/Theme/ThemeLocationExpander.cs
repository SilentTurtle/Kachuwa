using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;

namespace Kachuwa.Web.Theme
{
    public class ThemeLocationExpander : IKachuwaViewLocationExpander
    {


        public void PopulateValues(ViewLocationExpanderContext context)
        {
            //var tenant = (CurrentTenant)context.ActionContext.HttpContext.Items[TenantConstant.TenantContextKey];
            context.Values["themedir"] = "Themes";//theme.Directory;
            var themeconfig = context.ActionContext.HttpContext.RequestServices.GetService<IThemeConfig>();
            //temporary for single site
            string themeName = "";
            var area = context.ActionContext.RouteData.Values["area"];

            //no context when loading partial views
            
                if (area != null)
                {
                    themeName = themeconfig.BackendThemeName;
                }
                else
                {
                    themeName = themeconfig.FrontendThemeName;
                }

                context.Values["themename"] = themeName;
            
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