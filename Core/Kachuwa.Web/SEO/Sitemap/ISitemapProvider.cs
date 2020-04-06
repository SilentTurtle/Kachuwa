using System.Collections.Generic;
namespace Kachuwa.Web
{
    public interface ISitemapProvider
    {
        string CreateSitemap(IEnumerable<SitemapNode> nodes);

    }
}