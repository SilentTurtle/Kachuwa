using System.Threading.Tasks;
using Kachuwa.Data;
using Kachuwa.Web.Model;

namespace Kachuwa.Web
{
    public interface ISeoService
    {
        CrudService<SEO> Seo { get; set; }

        Task<bool> CheckUrlExist(string url, string type);
        Task<SEO> GetDetailByUrl(string url, string type);
        string GenerateForProduct(SEO productInfo);
        string GenerateMetaContents();
        Task<SEO> GetByProductId(int productId);
        string GetPage();
        Task<SEO> GetBySeoType(string seoType, int id);
    }
}