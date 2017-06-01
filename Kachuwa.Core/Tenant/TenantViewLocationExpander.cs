using System.Collections.Generic;
using System.Linq;
using Kachuwa.Web.Theme;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;

namespace Kachuwa.Tenant
{
    public class TenantViewLocationExpander : IViewLocationExpander
    {
        public void PopulateValues(ViewLocationExpanderContext context)
        {
            var tenant = (CurrentTenant)context.ActionContext.HttpContext.Items[TenantConstant.TenantContextKey];
            string tenantName = tenant.Info.Name;
            var area = context.ActionContext.RouteData.Values["area"];

            //no context when loading partial views
            if (tenantName != null)
            {
                context.Values["CurrentTenant"] = tenantName;
            }
        }

        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            string tenantName = null;
            if (context.Values.TryGetValue("CurrentTenant", out tenantName))
            {
                IEnumerable<string> tenantviewLocation = new[]
                {
                    $"/Views/{tenantName}/{{1}}/{{0}}.cshtml",
                    $"/Views/{tenantName}/Shared/{{0}}.cshtml"
                };

                //TODO::Area view location

                viewLocations = tenantviewLocation.Concat(viewLocations);
            }

            return viewLocations;
        }

    }
}