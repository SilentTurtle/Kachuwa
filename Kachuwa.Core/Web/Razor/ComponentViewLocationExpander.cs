using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Razor;

namespace Kachuwa.Web
{
    public class ComponentViewLocationExpander : IKachuwaViewLocationExpander
    {
        private const string _componentKey = "component";

        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            //if (context.Values.ContainsKey(_componentKey))
            //{
            //    var component = context.Values[_componentKey];
            //    if (!string.IsNullOrWhiteSpace(component))
            //    {
            var componentViewLocation = new string[]
            {
                "{0}.cshtml"
                       
            };
            viewLocations = componentViewLocation.Concat(viewLocations);
            return viewLocations;
            //    }
            //}
            //return viewLocations;
        }

        public void PopulateValues(ViewLocationExpanderContext context)
        {
            //var controller = context.ActionContext.ActionDescriptor.DisplayName;
            //var moduleName = controller.Split('.')[2];
            //if (moduleName != "WebHost")
            //{
            //    context.Values[_moduleKey] = moduleName;
            //}
        }
    }
}