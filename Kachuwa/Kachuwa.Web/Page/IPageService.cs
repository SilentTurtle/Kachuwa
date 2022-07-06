using System.Collections.Generic;
using System.Threading.Tasks;
using Kachuwa.Data;
using Kachuwa.Web.Layout;

namespace Kachuwa.Web
{
    public interface IPageService
    {
        CrudService<Page> CrudService { get; set; }
        CrudService<PagePermission> PermissionCrudService { get; set; }
        Task<bool> CheckPageExist(string url);
        Task<bool> Save(PageViewModel model);
        Task<PageViewModel> Get(int pageId);
        string GetPageNamespaces(bool includeMasterLayout);
        Task<bool> SavePageLayout(LayoutContent content);
        Task<bool> DeletePageAsync(long pageId);
        Task<bool> MakeLandingPage(long pageId);

        Task<IEnumerable<PagePermissionViewModel>> GetPermissionsFromCache();
        Task<IEnumerable<PagePermissionViewModel>> GetAllPermissions();
        Task<IEnumerable<PagePermissionViewModel>> GetPagePermission(int pageId);
        Task<bool> AddUpdatePagePermission(PageRolePermissionViewModel models);
    }
}