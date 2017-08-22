using System.Threading.Tasks;
using Kachuwa.Data;
using Kachuwa.Web.Layout;

namespace Kachuwa.Web
{
    public interface IPageService
    {
        CrudService<Page> CrudService { get; set; }
        Task<bool> CheckPageExist(string url);

        string GetPageNamespaces(bool includeMasterLayout);

        Task<bool> SavePageLayout(LayoutContent content);
    }
}