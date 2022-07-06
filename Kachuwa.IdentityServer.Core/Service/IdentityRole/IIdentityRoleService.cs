
using System.Collections.Generic;
using System.Threading.Tasks;
using Kachuwa.Data;
using IdentityUser = Kachuwa.IdentityServer.AspNetUsers;
using IdentityRole = Kachuwa.IdentityServer.AspNetRoles;

namespace Kachuwa.IdentityServer.Service
{ 
    public interface IIdentityRoleService
    {
        CrudService<IdentityRole> RoleService { get; set; }

        Task<IEnumerable<IdentityRole>> GetUserRolesAsync(long identityUserId);
        Task<bool> CheckNameExist(string roleName);
        Task<List<int>> GetRoleIds(string[] roleNames);
    }
}