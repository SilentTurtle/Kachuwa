using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Razor;

namespace Kachuwa.Web
{
    public class ViewOverrideLocationExpander : IKachuwaViewLocationExpander
    {
        public void PopulateValues(ViewLocationExpanderContext context)
        {
            var area =
                context.ActionContext.ActionDescriptor.RouteValues.FirstOrDefault(rc => rc.Key == "area");
            context.Values["ovr-area"] = area.Value;
        }

        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            var area = context.Values["ovr-area"] != null ? context.Values["ovr-area"].ToString() : "";
         
            IEnumerable<string> themeLocations = new[]
            {
                $"/Overrides/{area}/{{1}}/{{0}}.cshtml",
                $"/Overrides/{area}/Shared/{{0}}.cshtml",
                $"/Overrides/{{1}}/{{0}}.cshtml",
                $"/Overrides/Shared/{{0}}.cshtml"
            };
           
            //controller/action
            //shared/partial

            return themeLocations.Concat(viewLocations);
        }
    }
}