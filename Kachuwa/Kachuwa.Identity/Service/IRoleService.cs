
using System.Collections.Generic;
using System.Threading.Tasks;
using Kachuwa.Data;
using Kachuwa.Identity.Models;

namespace Kachuwa.Identity.Service
{ 
    public interface IIdentityRoleService
    {
        CrudService<IdentityRole> RoleService { get; set; }

        Task<IEnumerable<IdentityRole>> GetUserRolesAsync(long identityUserId);
        Task<bool> CheckNameExist(string roleName);
        Task<List<int>> GetRoleIds(string[] roleNames);
    }
}