using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Razor;

namespace Kachuwa.Web
{
    public class AreaViewLocationExpander : IKachuwaViewLocationExpander
    {
        public void PopulateValues(ViewLocationExpanderContext context)
        {
            // nothing here
        }

        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            var area =
                context.ActionContext.ActionDescriptor.RouteValues.FirstOrDefault(rc => rc.Key == "area");
            var additionalLocations = new LinkedList<string>();
            //TODO:: check for null
            //if (area !=null)
            //{
            additionalLocations.AddLast($"/Views/{area.Key}" + "{1}/{0}.cshtml");
            // }
            return viewLocations.Concat(additionalLocations);
        }
    }
}