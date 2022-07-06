using System.Collections.Generic;

namespace Kachuwa.Web
{
    public interface ISiteMapBuilder
    {
        IEnumerable<SitemapNode> Build();
    }
}