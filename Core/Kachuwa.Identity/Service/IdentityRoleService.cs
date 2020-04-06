using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using Dapper;
using Kachuwa.Data;
using Kachuwa.Identity.Models;

namespace Kachuwa.Identity.Service
{
    public class IdentityRoleService : IIdentityRoleService
    {
        public CrudService<IdentityRole> RoleService { get; set; } = new CrudService<IdentityRole>();
        public async Task<IEnumerable<IdentityRole>> GetUserRolesAsync(long identityUserId)
        {
            var dbFactory = DbFactoryProvider.GetFactory();
            using (var db = (DbConnection)dbFactory.GetConnection())
            {
                await db.OpenAsync();
                return await db.QueryAsync<IdentityRole>("select r.* from [dbo].[IdentityRole] as r inner join[dbo].[IdentityUserRole] as iur on iur.RoleId = r.Id "
                + " Where iur.UserId = @identityUserId",
                    new { identityUserId });
             
            }
        }

        public async Task<bool> CheckNameExist(string roleName)
        {
           var role= await this.RoleService.GetAsync("Where Name=@roleName", new {roleName});
            return role != null;
        }
    }


}