using System.Data.Common;
using System.Threading.Tasks;
using Dapper;
using Kachuwa.Data;
using Kachuwa.Identity.Models;

namespace Kachuwa.Identity.Service
{
    public class IdentityUserService: IIdentityUserService
    {
        public CrudService<IdentityUser> UserService { get; set; }=new CrudService<IdentityUser>();
        public CrudService<IdentityLogin> LoginService { get; set; }=new CrudService<IdentityLogin>();
        public CrudService<IdentityUserClaim> ClaimService { get; set; }=new CrudService<IdentityUserClaim>();
        public CrudService<IdentityUserRole> UserRoleService { get; set; }=new CrudService<IdentityUserRole>();
        public async Task<bool> AddUserRoles(int[] roleIds, long userId)
        {

            var dbFactory = DbFactoryProvider.GetFactory();
            using (var db = (DbConnection)dbFactory.GetConnection())
            {
                await db.OpenAsync();
                await db.ExecuteAsync("Delete  from dbo.IdentityUserRole Where UserId=@UserId;", new {UserId = userId});
                foreach (int roleId in roleIds)
                {
                    await db.ExecuteAsync("Insert into dbo.IdentityUserRole( UserId,RoleId )values(@UserId,@RoleId)",
                   new { UserId = userId, RoleId = roleId });
                }

                return true;
            }
        }
    }

}