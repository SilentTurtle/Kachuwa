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
        CrudService<MenuType> TypeCrudService { get; set; }
        CrudService<MenuSetting> SettingCrudService { get; set; }
        Task<int> SaveMenu(MenuViewModel model);
        Task<IEnumerable<MenuPermissionViewModel>> GetPermissionsFromCache();
        Task<IEnumerable<MenuPermissionViewModel>> GetAllPermissions();
    }
}