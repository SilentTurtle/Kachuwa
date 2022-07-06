using System.Collections.Generic;
using System.Threading.Tasks;
using Kachuwa.Data;
using Kachuwa.Web.ViewModels;
using NPOI.SS.Formula.PTG;

namespace Kachuwa.Web.Service
{
    public interface IPermissionService
    {
        CrudService<MasterRolePagePermission> RoleMasterPermissionCrudService { get; set; }
        CrudService<PagePermission> PagePermissionCrudService { get; set; }
        Task SaveRolePermissions(RoleWithPagePermission model);
        Task<bool> UpdateRolePermissionsOnly(UpdateRoleWithPagePermission model);
        Task DeleteRolePermissions(int roleId, long deletedBy);
        Task SaveUserPermissions(PageWithPagePermission model);
        Task DeleteUserPermissions(long userId, long deletedBy);
        Task RemoveUserSpecificPermissions(long userId,string permissionKey,bool allow);
        Task AddUserSpecificPermissions(long userId, string permissionKey, bool allow);
        Task SetUserApprovalLevelPermissions(long userId, int approveLevel);
        Task<IEnumerable<MasterRolePagePermission>> GetAllRoleBasedPermissionsAsync();
        Task<IEnumerable<PagePermission>> GetAllUserBasedPermissionsAsync();
    }
}