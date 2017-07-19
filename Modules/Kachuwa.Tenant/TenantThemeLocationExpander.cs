using System.Collections.Generic;
using System.Linq;
using Kachuwa.Web;
using Kachuwa.Web.Theme;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;

namespace Kachuwa.Tenant
{
    public class TenantThemeLocationExpander : IKachuwaViewLocationExpander
    {


        public void PopulateValues(ViewLocationExpanderContext context)
        {
            //TODO:: for multitenat setup
            var tenant = (CurrentTenant)context.ActionContext.HttpContext.Items[TenantConstant.TenantContextKey];
            context.Values["themedir"] = "Themes";//theme.Directory;

            var themeconfig = context.ActionContext.HttpContext.RequestServices.GetService<IThemeConfig>();
            //temporary for single site
            string themeName = "";
            var area = context.ActionContext.RouteData.Values["area"];

            //no context when loading partial views
            if (tenant != null)
            {
                if (area != null)
                {
                    themeName = tenant.Info.ThemeConfig.BackendThemeName; //themeconfig.BackendThemeName;
                }
                else
                {
                    themeName = tenant.Info.ThemeConfig.FrontendThemeName; //themeconfig.FrontendThemeName;
                }

                context.Values["themename"] = themeName;
            }
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
            var currentTenant = (CurrentTenant)context.ActionContext.HttpContext.Items[TenantConstant.TenantContextKey];
            //only for layout file look up
            IEnumerable<string> themeLocations = new[]
            {
                // $"/Themes/{theme}/Views/{{1}}/{{0}}.cshtml",
                $"/Themes/{currentTenant.Info.Name}/{theme}/Views/Shared/{{0}}.cshtml",
                $"/Themes/Shared/{theme}/Views/Shared/{{0}}.cshtml"
            };
            //default view path must be preserved other wise components wont load
            viewLocations = themeLocations.Concat(viewLocations);
            return viewLocations;

            //return viewLocations.Select(f => f.Replace("/Views/", "/" + context.Values["themedir"] +"/"+ context.Values["themename"]+ "/Views/"));

        }
    }
}