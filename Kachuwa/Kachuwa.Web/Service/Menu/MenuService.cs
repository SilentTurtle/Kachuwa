using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;
using Kachuwa.Caching;
using Kachuwa.Data;
using Kachuwa.Web.Model;
using Kachuwa.Web.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Dapper;
using Kachuwa.Data.Extension;

namespace Kachuwa.Web.Service
{

    public class MenuService : IMenuService
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ICacheService _cacheService;
        public CrudService<Menu> MenuCrudService { get; set; } = new CrudService<Menu>();
        public CrudService<MenuGroup> GroupCrudService { get; set; } = new CrudService<MenuGroup>();
        public CrudService<MenuSetting> SettingCrudService { get; set; } = new CrudService<MenuSetting>();
        public CrudService<MenuPermission> PermissionCrudService { get; set; } = new CrudService<MenuPermission>();
        private string _cacheKey = "Kachuwa.MenuPermissions";

        public MenuService(IWebHostEnvironment hostingEnvironment, ICacheService cacheService)
        {
            _hostingEnvironment = hostingEnvironment;
            _cacheService = cacheService;
        }

        public async Task<IEnumerable<MenuPermissionViewModel>> GetPermissionsFromCache()
        {
            var permissions = await _cacheService.GetAsync<IEnumerable<MenuPermissionViewModel>>(_cacheKey, async () =>
            {
                return await GetAllPermissions();
            });
            return permissions;
        }

        public async Task<IEnumerable<MenuPermissionViewModel>> GetAllPermissions()
        {
            var dbFactory = DbFactoryProvider.GetFactory();
            using (var db = (DbConnection)dbFactory.GetConnection())
            {
                await db.OpenAsync();
                return await db.QueryAsync<MenuPermissionViewModel>("select mp.*,m.Name,m.Url,ir.Name as RoleName from menupermission as mp " +
                    " inner join Menu as m on m.MenuId=mp.MenuId " +
                    " left join IdentityRole as ir on mp.RoleId=ir.Id ;");
            }
        }

        public async Task<bool> SaveMenuOrder(List<MenuOrderViewModel> orders)
        {
            var dbFactory = DbFactoryProvider.GetFactory();
            using (var db = (DbConnection)dbFactory.GetConnection())
            {
                await db.OpenAsync();
                foreach (var order in orders)
                {
                    await db.ExecuteAsync("Update Menu set MenuOrder=@MenuOrder,ParentId=@ParentId where MenuId=@MenuId",
                        new { MenuOrder = order.MenuOrder, MenuId = order.MenuId, ParentId= order.ParentId });
                }
                return true;
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
                    permission.MenuId = menuId;
                    permission.AutoFill();
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
                    permission.MenuId = model.MenuId;
                    permission.AutoFill();
                    await PermissionCrudService.InsertAsync<int>(permission);
                }
                _cacheService.Remove(_cacheKey);
                await GetPermissionsFromCache();
                return model.MenuId;

            }
        }

        public async Task<IEnumerable<MenuViewModel>> GetFooterMenu()
        {
            var dbFactory = DbFactoryProvider.GetFactory();
            using (var db = (DbConnection)dbFactory.GetConnection())
            {
                await db.OpenAsync();
                return await db.QueryAsync<MenuViewModel>("select * from Menu where menugroupid = @menuGroupId and isactive = 1", new { menuGroupId = 3 });
            }
        }
        public async Task<IEnumerable<MenuViewModel>> GetAdminMenus(IEnumerable<string> roles)
        {
            var dbFactory = DbFactoryProvider.GetFactory();
            using (var db = (DbConnection)dbFactory.GetConnection())
            {
                await db.OpenAsync();
                return await db.QueryAsync<MenuViewModel>(@"select distinct m.* from Menu m
                inner join MenuPermission mp on mp.MenuId = m.MenuId
                where m.IsBackend=@IsBackend and m.IsActive=@IsActive and  RoleId in (Select Id from identityRole where name in (Select items from dbo.udf_Split(@Roles, ','))) and AllowAccess = @AllowAccess",
                    new { IsBackend = true, IsActive = true, AllowAccess = true, Roles = string.Join(',', roles) });
            }
        }
    }
}
