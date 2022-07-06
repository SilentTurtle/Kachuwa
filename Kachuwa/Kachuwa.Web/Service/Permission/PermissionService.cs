using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Kachuwa.Caching;
using Kachuwa.Data;
using Kachuwa.Data.Extension;
using Kachuwa.Web.ViewModels;

namespace Kachuwa.Web.Service
{
    public class PermissionService : IPermissionService
    {
        private string _cacheKey = "Kachuwa.PagePermissionExtended";
        public CrudService<MasterRolePagePermission> RoleMasterPermissionCrudService { get; set; } = new CrudService<MasterRolePagePermission>();
        public CrudService<PagePermission> PagePermissionCrudService { get; set; } = new CrudService<PagePermission>();
        private readonly ICacheService _cacheService;
        public PermissionService(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }
        public async Task SaveRolePermissions(RoleWithPagePermission model)
        {
            await DeleteRolePermissions((int)model.Id, model.AddedBy);
            foreach (var permission in model.Permissions)
            {
                permission.AutoFill();
                permission.RoleId = model.Id;
                permission.AutoFill();
                await RoleMasterPermissionCrudService.InsertAsync(permission);
            }
        }

        public async Task DeleteRolePermissions(int roleId, long deletedBy)
        {
            await RoleMasterPermissionCrudService.UpdateAsDeleted("Where RoleId=@RoleId", new { DeletedBy = deletedBy, RoleId = roleId });
        }

        public async Task SaveUserPermissions(PageWithPagePermission model)
        {
            await DeleteUserPermissions(model.UserId, model.AddedBy);
            foreach (var permission in model.Permissions)
            {
                permission.AutoFill();
                permission.UserId = model.UserId;
                permission.AutoFill();
                await PagePermissionCrudService.InsertAsync(permission);
            }
        }

        public async Task DeleteUserPermissions(long userId, long deletedBy)
        {
            throw new NotImplementedException();
        }

        public Task RemoveUserSpecificPermissions(long userId, string permissionKey, bool allow)
        {
            throw new NotImplementedException();
        }

        public Task AddUserSpecificPermissions(long userId, string permissionKey, bool allow)
        {
            throw new NotImplementedException();
        }

        public Task SetUserApprovalLevelPermissions(long userId, int approveLevel)
        {
            throw new NotImplementedException();

        }

        public async Task<IEnumerable<MasterRolePagePermission>> GetAllRoleBasedPermissionsAsync()
        {

            var dbFactory = DbFactoryProvider.GetFactory();
            using (var db = (DbConnection)dbFactory.GetConnection())
            {
                await db.OpenAsync();
                return await db.QueryAsync<MasterRolePagePermission>(@"select mp.*,p.URL from dbo.MasterRolePagePermission
mp inner join dbo.page p on p.PageId=mp.PageId",
                    new { });
            }
        }
        public async Task<IEnumerable<PagePermission>> GetAllUserBasedPermissionsAsync()
        {
            var dbFactory = DbFactoryProvider.GetFactory();
            using (var db = (DbConnection)dbFactory.GetConnection())
            {
                await db.OpenAsync();
                return await db.QueryAsync<PagePermission>(@"select mp.*,p.URL from dbo.PagePermission
mp inner join dbo.page p on p.PageId=mp.PageId",
                    new { });
            }
        }

        public async Task<bool> CheckPermission(int userId, int pageCode, string action)
        {

            return true;
        }

        public async Task<bool> UpdateRolePermissionsOnly(UpdateRoleWithPagePermission model)
        {
            await DeleteRolePermissions((int)model.RoleId, model.AddedBy);
            foreach (var permission in model.Permissions)
            {
                permission.RoleId = model.RoleId;
                permission.AutoFill();
                if (permission.MasterRolePagePermissionId == 0)
                {
                    await RoleMasterPermissionCrudService.InsertAsync(permission);
                }
                else
                {

                    await RoleMasterPermissionCrudService.UpdateAsync(permission);
                }

            }
            return true;
        }
    }
}
