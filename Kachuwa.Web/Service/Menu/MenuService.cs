using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;
using Kachuwa.Caching;
using Kachuwa.Data;
using Kachuwa.Data.Crud.FormBuilder;
using Kachuwa.Web.Model;
using Kachuwa.Web.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Dapper;

namespace Kachuwa.Web.Service
{

    public class MenuService : IMenuService
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly ICacheService _cacheService;

        public CrudService<Menu> MenuCrudService { get; set; } = new CrudService<Menu>();
        public CrudService<MenuType> TypeCrudService { get; set; } = new CrudService<MenuType>();
        public CrudService<MenuSetting> SettingCrudService { get; set; } = new CrudService<MenuSetting>();
        public CrudService<MenuPermission> PermissionCrudService { get; set; } = new CrudService<MenuPermission>();
        private string _cacheKey = "Kachuwa.Permissions";

        public MenuService(IHostingEnvironment hostingEnvironment, ICacheService cacheService)
        {
            _hostingEnvironment = hostingEnvironment;
            _cacheService = cacheService;
        }

        public async Task<IEnumerable<MenuPermissionViewModel>> GetPermissionsFromCache()
        {
            var permissions = await _cacheService.GetAsync<IEnumerable<MenuPermissionViewModel>>(_cacheKey, async () =>
            {
                return await GetAllPermissions() ;
            });
            return permissions;
        }

        public  async Task<IEnumerable<MenuPermissionViewModel>> GetAllPermissions()
        {
            var dbFactory = DbFactoryProvider.GetFactory();
            using (var db = (DbConnection)dbFactory.GetConnection())
            {
                await db.OpenAsync();
                return await db.QueryAsync<MenuPermissionViewModel>("select mp.*,m.Name,m.Url,ir.Name as RoleName from dbo.menupermission as mp " +
                    " inner join dbo.Menu as m on m.MenuId=mp.MenuId " +
                    " left join dbo.IdentityRole as ir on mp.RoleId=ir.Id ;");
            }
        }

        public async Task<int> SaveMenu(MenuViewModel model)
        {
            if (model.MenuId == 0)
            {
                model.AutoFill();
                var menuId = await MenuCrudService.InsertAsync<int>(model);
                foreach (var permission in model.Permissions)
                {
                    await PermissionCrudService.InsertAsync<int>(permission);
                }
                _cacheService.Remove(_cacheKey);
                await GetPermissionsFromCache();
                return menuId;
            }
            else
            {
                model.AutoFill();
                await MenuCrudService.UpdateAsync(model);
                await PermissionCrudService.DeleteAsync("Where MenuId=@MenuId", new { model.MenuId });
                foreach (var permission in model.Permissions)
                {
                    await PermissionCrudService.InsertAsync<int>(permission);
                }
                _cacheService.Remove(_cacheKey);
                await GetPermissionsFromCache();
                return model.MenuId;

            }
        }
    }
}
