using System;
using System.Data;
using System.Data.Common;
using Dapper;
using Kachuwa.Data;
using Microsoft.AspNetCore.Identity;
using IdentityUser = Kachuwa.IdentityServer.AspNetUsers;
using IdentityRole = Kachuwa.IdentityServer.AspNetRoles;
using IdentityLogin = Kachuwa.IdentityServer.AspNetUserLogins;
using IdentityUserClaim = Kachuwa.IdentityServer.AspNetUserClaims;
using IdentityUserRole = Kachuwa.IdentityServer.AspNetUserRoles;

namespace Kachuwa.IdentityServer.Service
{
    public class IdentityUserService : IIdentityUserService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        
        public CrudService<IdentityUser> UserService { get; set; } = new CrudService<IdentityUser>();
        public CrudService<IdentityLogin> LoginService { get; set; } = new CrudService<IdentityLogin>();
        public CrudService<IdentityUserClaim> ClaimService { get; set; } = new CrudService<IdentityUserClaim>();
        public CrudService<IdentityUserRole> UserRoleService { get; set; } = new CrudService<IdentityUserRole>();

        public IdentityUserService()
        {
           
        }

        public async Task<IdentityLogin> GetExternalUserAsync(string loginProvider, string providerKey)
        {
            var dbFactory = DbFactoryProvider.GetFactory();
            using (var db = (DbConnection)dbFactory.GetConnection())
            {
                await db.OpenAsync();
                return await db.QuerySingleOrDefaultAsync<IdentityLogin>("select * from  IdentityUserLogin Where LoginProvider=@LoginProvider and ProviderDisplayName=@ProviderDisplayName and  ProviderKey=@ProviderKey",
                     new { LoginProvider = loginProvider, ProviderDisplayName = "ProviderUser", ProviderKey = providerKey });


            }
        }
        public async Task<bool> AddUserLoginProvider(IdentityLogin model)
        {

            var dbFactory = DbFactoryProvider.GetFactory();
            using (var db = (DbConnection)dbFactory.GetConnection())
            {
                await db.OpenAsync();
                await db.ExecuteAsync("insert into IdentityUserLogin (LoginProvider,ProviderDisplayName,ProviderKey,UserId) values(@LoginProvider,@ProviderDisplayName,@ProviderKey,@UserId)", new { model.LoginProvider, model.ProviderDisplayName, model.ProviderKey, model.UserId });

                return true;
            }
        }
        public async Task<bool> DeleteAllUserLoginProviders(long userId)
        {

            var dbFactory = DbFactoryProvider.GetFactory();
            using (var db = (DbConnection)dbFactory.GetConnection())
            {
                await db.OpenAsync();
                await db.ExecuteAsync("delete from IdentityUserLogin Where UserId=@UserId", new { UserId = userId });

                return true;
            }
        }
        public async Task<bool> DeleteUserLoginProvider(long userId, string provider)
        {

            var dbFactory = DbFactoryProvider.GetFactory();
            using (var db = (DbConnection)dbFactory.GetConnection())
            {
                await db.OpenAsync();
                await db.ExecuteAsync("delete from IdentityUserLogin Where UserId=@UserId and LoginProvider=@LoginProvider ", new { LoginProvider = provider, UserId = userId });

                return true;
            }
        }

        public async Task<bool> AddUserRoles(int[] roleIds, long userId)
        {

            var dbFactory = DbFactoryProvider.GetFactory();
            using (var db = (DbConnection)dbFactory.GetConnection())
            {
                await db.OpenAsync();
                await db.ExecuteAsync("Delete  from IdentityUserRole Where UserId=@UserId;", new { UserId = userId });
                foreach (int roleId in roleIds)
                {
                    await db.ExecuteAsync("Insert into IdentityUserRole( UserId,RoleId )values(@UserId,@RoleId)",
                   new { UserId = userId, RoleId = roleId });
                }

                return true;
            }
        }

        public async Task<bool> AddUserRoles(int[] roleIds, long userId, IDbConnection db, IDbTransaction tran)
        {

            await db.ExecuteAsync("Delete from IdentityUserRole Where UserId=@UserId;", new { UserId = userId }, tran);
            foreach (int roleId in roleIds)
            {
                await db.ExecuteAsync("Insert into IdentityUserRole( UserId,RoleId )values(@UserId,@RoleId)",
                    new { UserId = userId, RoleId = roleId }, tran);
            }

            return true;

        }

        public async Task DeleteUserRoles(long identityUserId)
        {
            var dbFactory = DbFactoryProvider.GetFactory();
            using (var db = (DbConnection)dbFactory.GetConnection())
            {
                await db.OpenAsync();
                await db.ExecuteAsync("Delete from IdentityUserRole Where UserId=@UserId;", new { UserId = identityUserId });


            }
        }



        public async Task<List<int>> GetUserRoles(long userIdentityUserId)
        {
            var dbFactory = DbFactoryProvider.GetFactory();
            using (var db = (DbConnection)dbFactory.GetConnection())
            {
                await db.OpenAsync();
                var roles = await db.QueryAsync<int>("Select RoleId from IdentityUserRole Where UserId=@UserId;", new { UserId = userIdentityUserId });
                return roles.ToList();


            }
        }



    }

}