using System;
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

    public class SeoService : ISeoService
    {
        public CrudService<SEO> Seo { get; set; }=new CrudService<SEO>();

        public Task<bool> CheckUrlExist(string url, string type)
        {
            throw new NotImplementedException();
        }

        public string GenerateForProduct(SEO productInfo)
        {
            throw new NotImplementedException();
        }

        public string GenerateMetaContents()
        {
            throw new NotImplementedException();
        }

        public Task<SEO> GetByProductId(int productId)
        {
            throw new NotImplementedException();
        }

        public Task<SEO> GetBySeoType(string seoType, int id)
        {
            throw new NotImplementedException();
        }

        public Task<SEO> GetDetailByUrl(string url, string type)
        {
            throw new NotImplementedException();
        }

        public string GetPage()
        {
            throw new NotImplementedException();
        }
    }
}