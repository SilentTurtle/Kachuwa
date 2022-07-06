using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Kachuwa.Web
{
    public class ControllerSiteMapBuilder : ISiteMapBuilder
    {
        private string BaseCategoryUrl { get; set; }
        private string BaseNewsUrl { get; set; }
        private string BaseVideoUrl { get; set; }
        private string BaseImageUrl { get; set; }

        //readonly UrlHelper _urlHelper=new UrlHelper(HttpContext.Current.Request.RequestContext);
        public IEnumerable<SitemapNode> Build()
        {
            List<SitemapNode> nodes = new List<SitemapNode>();
            nodes.Add(
                new SitemapNode()
                {
                    //Url = _urlHelper.AbsoluteRouteUrl("index", "Home"),
                    Priority = 1
                });
            return nodes;
        }

    }
}