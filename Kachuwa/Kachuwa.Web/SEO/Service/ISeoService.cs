using System.Threading.Tasks;
using Kachuwa.Data;
using Kachuwa.Web.Model;

namespace Kachuwa.Web
{
    public interface ISeoService
    {
        CrudService<SEO> Seo { get; set; }
        Task<bool> CheckUrlExist(string url, string type);
        Task<string> GenerateMetaContents();
        Task<SEO> GetBySeoType(string seoType, int id);
        Task<string> GenerateJsonLdForWebSite();
        Task<string> GenerateJsonLdForPage();
        Task<SEO> GetByProductId(int producId, string type);
        Task<string> GenerateJsonLdForPage(string page,int productId,string type);
        Task<string> GetSEOMetaContentsAsync(string url, string type);
        Task<SEO> GetSEODataAsync(string url, string type);
    }
}