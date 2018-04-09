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
            
            var componentViewLocation = new string[]
            {
                "{0}.cshtml"
                       
            };
            viewLocations = componentViewLocation.Concat(viewLocations);
            return viewLocations;
           
        }

        public void PopulateValues(ViewLocationExpanderContext context)
        {
        }
    }
}