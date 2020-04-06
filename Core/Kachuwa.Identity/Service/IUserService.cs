using System.Collections.Generic;
using System.Data;
using System.Security.Claims;
using System.Threading.Tasks;
using Kachuwa.Data;
using Kachuwa.Identity.Models;
using Kachuwa.Identity.ViewModels;

namespace Kachuwa.Identity.Service
{
    public interface IIdentityUserService
    {
        CrudService<IdentityUser> UserService { get; set; }
        CrudService<IdentityLogin> LoginService { get; set; }
        CrudService<IdentityUserClaim> ClaimService { get; set; }
        CrudService<IdentityUserRole> UserRoleService { get; set; }

        Task<bool> AddUserLoginProvider(IdentityLogin model);
        Task<bool> DeleteAllUserLoginProviders(long userId);
        Task<bool> DeleteUserLoginProvider(long userId, string provider);
        Task<bool> AddUserRoles(int[] roleIds,long userId);
        Task<bool> AddUserRoles(int[] roleIds, long userId,IDbConnection db,IDbTransaction tran);
        Task<List<int>> GetUserRoles(long userIdentityUserId);
        Task DeleteUserRoles(long identityUserId);

        Task<IdentityLogin> GetExternalUserAsync(string loginProvider, string providerKey);
    }
}