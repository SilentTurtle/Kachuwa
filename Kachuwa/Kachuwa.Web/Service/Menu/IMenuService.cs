using Kachuwa.Data;
using Kachuwa.Web.Model;
using Kachuwa.Web.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kachuwa.Web.Service
{
    public interface IMenuService
    {
        CrudService<Menu> MenuCrudService { get; set; }
        CrudService<MenuPermission> PermissionCrudService { get; set; }
        CrudService<MenuGroup> GroupCrudService { get; set; }
        CrudService<MenuSetting> SettingCrudService { get; set; }
        Task<int> SaveMenu(MenuViewModel model);
        Task<IEnumerable<MenuPermissionViewModel>> GetPermissionsFromCache();
        Task<IEnumerable<MenuPermissionViewModel>> GetAllPermissions();
        Task<bool> SaveMenuOrder(List<MenuOrderViewModel> orders);

        Task<IEnumerable<MenuViewModel>> GetFooterMenu();
        Task<IEnumerable<MenuViewModel>> GetAdminMenus(IEnumerable<string> roles);
    }
}