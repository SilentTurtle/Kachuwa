using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Duende.IdentityServer.Test;
using IdentityModel;
using Kachuwa.Data;
using Kachuwa.Data.Extension;
using Kachuwa.Extensions;
using Kachuwa.IdentityServer;
using Kachuwa.IdentityServer.ViewModel;
using Kachuwa.Storage;
using Microsoft.AspNetCore.Identity;
using IdentityUser = Kachuwa.IdentityServer.AspNetUsers;
using IdentityRole = Kachuwa.IdentityServer.AspNetRoles;
using IdentityLogin = Kachuwa.IdentityServer.AspNetUserLogins;
using IdentityUserClaim = Kachuwa.IdentityServer.AspNetUserClaims;
using IdentityUserRole = Kachuwa.IdentityServer.AspNetUserRoles;

namespace Kachuwa.IdentityServer.Service
{
    public class AppUserService : IAppUserService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IIdentityUserService _identityUserService;
        private readonly IIdentityRoleService _identityRoleService;
        private readonly IStorageProvider _storageProvider;

        public AppUserService(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, 
            IIdentityUserService identityUserService, IIdentityRoleService identityRoleService,IStorageProvider storageProvider)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _identityUserService = identityUserService;
            _identityRoleService = identityRoleService;
            _storageProvider = storageProvider;
        }
        public CrudService<AppUser> AppUserCrudService { get; set; } = new CrudService<AppUser>();
      
        public async Task<UserStatus> SaveNewUserAsync(UserViewModel model)
        {
            var status = new UserStatus();
            try
            {
                if (model.AppUserId == 0)
                {
                    var user = new IdentityUser { UserName = model.UserName, Email = model.Email };
                    var existingUser = await _userManager.FindByEmailAsync(model.Email);
                    if (existingUser != null)
                    {
                        status.HasError = true;
                        status.Message = "Email is already registered";
                        return status;
                    }
                    var existingUser2 = await _userManager.FindByNameAsync(model.UserName);
                    if (existingUser2 != null)
                    {
                        status.HasError = true;
                        status.Message = "UserName is already registered";
                        return status;
                    }


                    var dbFactory = DbFactoryProvider.GetFactory();
                    using (var db = (DbConnection)dbFactory.GetConnection())
                    {
                        await db.OpenAsync();
                        using (var dbTran = db.BeginTransaction())
                        {
                            try
                            {
                                var stat = await _userManager.CreateAsync(user, model.Password);
                                if (stat.Succeeded)
                                {
                                    var identityUser = await _userManager.FindByEmailAsync(model.Email);
                                    var appUser = model.To<AppUser>();
                                    appUser.AutoFill();
                                    appUser.IdentityUserId = identityUser.Id;
                                    appUser.UserName = identityUser.UserName;
                                    appUser.IsActive = true;
                                    var id = await AppUserCrudService.InsertAsync<long>(db, appUser, dbTran, 10);

                                    var roleIds = model.UserRoles.Where(z => z.IsSelected == true).Select(x => (int)x.RoleId).ToArray();
                                    if (roleIds != null && roleIds.Any())
                                        await _identityUserService.AddUserRoles(roleIds, identityUser.Id);
                                    else
                                        await _identityUserService.AddUserRoles(new int[] { 3 }, identityUser.Id);//customer role

                                    dbTran.Commit();
                                    status.IdentityUserId = appUser.IdentityUserId;
                                    status.HasError = false;
                                    status.Message = id.ToString();
                                    return status;
                                }

                                status.HasError = true;
                                status.Message = string.Join(",", stat.Errors);
                                return status;
                            }
                            catch (Exception e)
                            {

                                dbTran.Rollback();
                                var identityUser = await _userManager.FindByEmailAsync(model.Email);
                                await _userManager.DeleteAsync(identityUser);
                                status.HasError = true;
                                status.Message = e.Message.ToString();
                                return status;
                            }
                        }





                    }
                    //status.HasError = true;
                    //status.Message = string.Join(",", result.Errors);
                    //return status;
                }
                else
                {
                    var appUser = model.To<AppUser>();
                    appUser.AutoFill();
                    var id = await AppUserCrudService.UpdateAsync(appUser);
                    //await _identityUserService.DeleteUserRoles(appUser.IdentityUserId );
                    var roleIds = model.UserRoles.Where(z => z.IsSelected == true).Select(x => (int)x.RoleId).ToArray();
                    await _identityUserService.AddUserRoles(roleIds, appUser.IdentityUserId);
                    status.HasError = false;
                    status.Message = "Updated Succesfully";
                    status.IdentityUserId = appUser.IdentityUserId;
                    return status;
                }
            }
            catch (Exception e)
            {
                status.HasError = true;
                status.Message = e.Message;
                return status;
            }
        }

        public async Task<UserStatus> UpdateUserAsync(UserEditViewModel model)
        {
            var status = new UserStatus();
            try
            {
                if (model.AppUserId != 0)
                {
                    var appUser = model.To<AppUser>();
                    appUser.AutoFill();
                    var id = await AppUserCrudService.UpdateAsync(appUser);
                    // await _identityUserService.DeleteUserRoles(appUser.IdentityUserId);
                    var roleIds = model.UserRoles.Where(z => z.IsSelected == true).Select(x => (int)x.RoleId).ToArray();
                    await _identityUserService.AddUserRoles(roleIds, appUser.IdentityUserId);

                    status.HasError = false;
                    status.Message = "Updated Succesfully";
                    return status;
                }
                status.HasError = true;
                status.Message = "Invalid AppUserId";
                return status;
            }
            catch (Exception e)
            {
                status.HasError = true;
                status.Message = e.Message;
                return status;
            }
        }

        public async Task<bool> DeleteUserAsync(int appUserId)
        {
            var user = await AppUserCrudService.GetAsync(appUserId);
            var identityUser = await _userManager.FindByEmailAsync(user.Email);
            if (identityUser != null)
                await _userManager.DeleteAsync(identityUser);

            user.IsDeleted = true;
            user.IsActive = false;
            await AppUserCrudService.UpdateAsync(user);
            return true;
        }

        public async Task<bool> AssignRolesAsync(UserRolesViewModel userRoles)
        {
            await _identityUserService.AddUserRoles(userRoles.Roles.Select(e => (int)e.Id).ToArray(), userRoles.IdentityUserId);
            return true;
        }

        public async Task<UserEditViewModel> GetAsync(int appuserId)
        {
            var user = await AppUserCrudService.GetAsync(appuserId);
            var userViewModel = user.To<UserEditViewModel>();
            userViewModel.UserRoleIds = await _identityUserService.GetUserRoles(user.IdentityUserId);
            return userViewModel;
        }

        public async Task<bool> UpdateProfilePicture(long identityUserId, string imagePath)
        {
            var dbFactory = DbFactoryProvider.GetFactory();
            using (var db = (DbConnection)dbFactory.GetConnection())
            {
                await db.OpenAsync();
                await db.ExecuteAsync("Update AppUser Set ProfilePicture=@Image where IdentityUserId=@identityUserId",
                    new { identityUserId, Image = imagePath });
                return true;
            }
        }

        public async Task<bool> UpdateUserDeviceId(long appUserId, string deviceId)
        {
            //TODO:: update table fields
            throw new NotImplementedException();
        }

        public async Task<int> GetNewUserStatusAsync(int showRecords)
        {
            var dbFactory = DbFactoryProvider.GetFactory();
            using (var db = (DbConnection)dbFactory.GetConnection())
            {
                await db.OpenAsync();
                return await db.ExecuteScalarAsync<int>(@"if(@showRecords=1)
                     select count(1) from appuser where cast(addedon as date)=cast(GETUTCDATE() as date)
                    else if(@showRecords=2)   
                    select count(1) from appuser where cast(addedon as date)= dateAdd(day, -1,cast(GETUTCDATE() as date))
                    else if(@showRecords=3 )
                    select count(1) from appuser where cast(addedon as date) between  DATEADD(DAY, 1 - DATEPART(WEEKDAY, GETUTCDATE()), CAST(GETUTCDATE() AS DATE)) and  DATEADD(DAY, 7 - DATEPART(WEEKDAY, GETUTCDATE()), CAST(GETUTCDATE() AS DATE)) 

", new { showRecords });
            }
        }

        public async Task<UserStatus> AddExternalUser(string provider, string providerUserId, List<Claim> claims)
        {

            // create a list of claims that we want to transfer into our store
            var filtered = new List<Claim>();

            foreach (var claim in claims)
            {
                // if the external system sends a display name - translate that to the standard OIDC name claim
                if (claim.Type == ClaimTypes.Name)
                {
                    filtered.Add(new Claim(JwtClaimTypes.Name, claim.Value));
                }
                // if the JWT handler has an outbound mapping to an OIDC claim use that
                else if (JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.ContainsKey(claim.Type))
                {
                    filtered.Add(new Claim(JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap[claim.Type], claim.Value));
                }
                // copy the claim as-is
                else
                {
                    filtered.Add(claim);
                }
            }

            // if no display name was provided, try to construct by first and/or last name
            if (!filtered.Any(x => x.Type == JwtClaimTypes.Name))
            {
                var first = filtered.FirstOrDefault(x => x.Type == JwtClaimTypes.GivenName)?.Value;
                var last = filtered.FirstOrDefault(x => x.Type == JwtClaimTypes.FamilyName)?.Value;
                if (first != null && last != null)
                {
                    filtered.Add(new Claim(JwtClaimTypes.Name, first + " " + last));
                }
                else if (first != null)
                {
                    filtered.Add(new Claim(JwtClaimTypes.Name, first));
                }
                else if (last != null)
                {
                    filtered.Add(new Claim(JwtClaimTypes.Name, last));
                }
            }

            // create a new unique subject id
            var sub = CryptoRandom.CreateUniqueId();

            // check if a display name is available, otherwise fallback to subject id
            var name = filtered.FirstOrDefault(c => c.Type == JwtClaimTypes.Name)?.Value ?? sub;

            // create new user
            var user = new TestUser
            {
                SubjectId = sub,
                Username = name,
                ProviderName = provider,
                ProviderSubjectId = providerUserId,
                Claims = filtered
            };


            // add user to in-memory store
            //  _users.Add(user);




            var model = new UserViewModel();
            model.Password = Guid.NewGuid().ToString();
            model.FirstName = filtered.FirstOrDefault(x => x.Type == JwtClaimTypes.GivenName)?.Value;
            model.LastName = filtered.FirstOrDefault(x => x.Type == JwtClaimTypes.FamilyName)?.Value;
            model.Email = filtered.FirstOrDefault(x => x.Type == JwtClaimTypes.Email)?.Value;
            model.ProfilePicture = filtered.FirstOrDefault(x => x.Type == JwtClaimTypes.Picture)?.Value;
            model.PhoneNumber = filtered.FirstOrDefault(x => x.Type == JwtClaimTypes.PhoneNumber)?.Value;
            model.IsActive = true;
            bool noEmail = string.IsNullOrEmpty(model.Email);
            bool noPhone = string.IsNullOrEmpty(model.PhoneNumber);
            if (noEmail)
            {
                model.Email = model.Password + "@kachuwaframework.com";

            }
            model.UserRoles = new List<UserRolesSelected>
            {
                new UserRolesSelected()
                {
                    IsSelected = true,
                    Name = KachuwaRoleNames.User,
                    RoleId = KachuwaRoles.User

                }
            };
            if (!string.IsNullOrEmpty(model.ProfilePicture))
            {
                using (System.Net.WebClient webClient = new System.Net.WebClient())
                {
                    using (Stream stream = webClient.OpenRead(model.ProfilePicture))
                    {
                        model.ProfilePicture = await _storageProvider.SaveFile("Profile", "image/jpeg", stream);
                    }
                }
              
            }
            var status = new UserStatus();
            try
            {

                var iuser = new IdentityUser
                {
                    UserName = noEmail == true && noPhone == false ? model.PhoneNumber : model.Email,
                    Email = model.Email
                };
                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    status.HasError = true;
                    status.Message = "Email is already registered";
                    return status;
                }

                var dbFactory = DbFactoryProvider.GetFactory();
                using (var db = (DbConnection)dbFactory.GetConnection())
                {
                    await db.OpenAsync();
                    using (var dbTran = db.BeginTransaction())
                    {
                        try
                        {
                            var stat = await _userManager.CreateAsync(iuser, model.Password);
                            if (stat.Succeeded)
                            {
                                var identityUser = await _userManager.FindByEmailAsync(model.Email);
                                var appUser = model.To<AppUser>();
                                appUser.AutoFill();
                                appUser.UserName = iuser.UserName;
                                appUser.IdentityUserId = identityUser.Id;
                                var id = await AppUserCrudService.InsertAsync<long>(db, appUser, dbTran, 10);

                                var roleIds = model.UserRoles.Where(z => z.IsSelected == true).Select(x => (int)x.RoleId).ToArray();
                                if (roleIds != null && roleIds.Any())
                                    await _identityUserService.AddUserRoles(roleIds, identityUser.Id);
                                else
                                    await _identityUserService.AddUserRoles(new int[] { 3 }, identityUser.Id);//customer role


                                var extLogin = new IdentityLogin()
                                {
                                    ProviderDisplayName = "ProviderUser",
                                    ProviderKey = providerUserId,
                                    LoginProvider = provider,
                                    UserId = appUser.IdentityUserId

                                };
                                await _identityUserService.AddUserLoginProvider(extLogin);

                                dbTran.Commit();
                                status.HasError = false;
                                status.Message = id.ToString();

                            }
                            status.HasError = true;
                            status.Message = string.Join(",", stat.Errors);
                            return status;

                        }
                        catch (Exception e)
                        {

                            dbTran.Rollback();
                            var identityUser = await _userManager.FindByEmailAsync(model.Email);
                            await _userManager.DeleteAsync(identityUser);
                            status.HasError = true;
                            status.Message = e.Message.ToString();
                            return status;

                        }
                    }





                }


            }
            catch (Exception e)
            {
                status.HasError = true;
                status.Message = e.Message;
                return status;
            }


        }

        public async Task<IEnumerable<UserExportViewModel>> ExportAllUsersAsync(int organizationId)
        {
            var dbFactory = DbFactoryProvider.GetFactory();
            using (var db = (DbConnection)dbFactory.GetConnection())
            {
                await db.OpenAsync();
                return await db.QueryAsync<UserExportViewModel>("select au.FirstName,au.LastName,au.Email,au.Username,au.PhoneNumber,o.CompanyName as Organization, au.GroupName from  AppUser au left join Organization as o on o.OrganizationId=au.OrganizationId" +
                                                                  " Where (o.OrganizationId=@OrganizationId or @OrganizationId=0) and o.IsDeleted = 0 order by au.FirstName asc",
                       new { OrganizationId = organizationId });

            }
        }

        public async Task<IEnumerable<UserExportViewModel>> ExportAllPackageUsersAsync(int organizationId, int packageId)
        {
            var dbFactory = DbFactoryProvider.GetFactory();
            using (var db = (DbConnection)dbFactory.GetConnection())
            {
                await db.OpenAsync();
                return await db.QueryAsync<UserExportViewModel>("select au.FirstName,au.LastName,au.Email,au.Username,au.PhoneNumber, au.GroupName,oo.CompanyName as Organization  from  AppUser au inner join  UserPackage as o on o.OrganizationId=au.OrganizationId and o.UserId=au.IdentityUserId " +
                                                                " left join Organization as oo on oo.OrganizationId=au.OrganizationId Where  PackageId=@packageId and  (o.OrganizationId=@OrganizationId or @OrganizationId=0) and o.IsDeleted = 0 order by au.FirstName asc",
                    new { OrganizationId = organizationId, packageId });

            }
        }
        public async Task<IEnumerable<UserPrintouViewModel>> GetAllPackageUser(int organizationId, int packageId)
        {
            var dbFactory = DbFactoryProvider.GetFactory();
            using (var db = (DbConnection)dbFactory.GetConnection())
            {
                await db.OpenAsync();
                return await db.QueryAsync<UserPrintouViewModel>("select au.IdentityUserId, au.FirstName,au.LastName,au.Email,au.Username,au.PhoneNumber, au.GroupName,oo.CompanyName as Organization,[dbo].[udf_GetUserPackageNames](au.IdentityUserId) as PackageNames,oo.CompanyName as Organization  from  AppUser au inner join  UserPackage as o on o.OrganizationId=au.OrganizationId and o.UserId=au.IdentityUserId " +
                                                                " left join Organization as oo on oo.OrganizationId=au.OrganizationId " +
                                                    "  Where  PackageId=@packageId and  (o.OrganizationId=@OrganizationId or @OrganizationId=0) and o.IsDeleted = 0 order by au.GroupName asc",
                    new { OrganizationId = organizationId, packageId });

            }
        }

        public async Task<string> GetUserEmailById(long customerId)
        {

            var dbFactory = DbFactoryProvider.GetFactory();
            using (var db = (DbConnection)dbFactory.GetConnection())
            {
                await db.OpenAsync();
                return await db.ExecuteScalarAsync<string>("select Email from AppUser where IdentityUserId=@IdentityUserId ",
                    new { IdentityUserId = customerId });

            }
        }

        public async Task<bool> ChangeEmailAsync(long userId, string email)
        {
            var dbFactory = DbFactoryProvider.GetFactory();
            using (var db = (DbConnection)dbFactory.GetConnection())
            {
                await db.OpenAsync();
                await db.ExecuteAsync("Update  [dbo].[AppUser] Set Email=@Email where IdentityUserId=@IdentityUserId ;" +
                                      "Update [dbo].[IdentityUser] Set Email=@Email,NormalizedEmail=@NEmail where Id=@IdentityUserId ;",
                   new { IdentityUserId = userId, Email = email, NEmail = email.ToUpper() });
                return true;
            }
        }
        public async Task<bool> ChangePhoneNumberAsync(long userId, string phoneNumber)
        {
            var dbFactory = DbFactoryProvider.GetFactory();
            using (var db = (DbConnection)dbFactory.GetConnection())
            {
                await db.OpenAsync();
                await db.ExecuteAsync("Update  [dbo].[AppUser] Set PhoneNumber=@PhoneNumber where IdentityUserId=@IdentityUserId ;",
                    new { IdentityUserId = userId, PhoneNumber = phoneNumber });
                await db.ExecuteAsync("Update  [dbo].[IdentityUser] Set PhoneNumber=@PhoneNumber where Id=@IdentityUserId ;",
               new { IdentityUserId = userId, PhoneNumber = phoneNumber });
                return true;
            }
        }
        public async Task<bool> CheckEmailExists(string email)
        {
            var dbFactory = DbFactoryProvider.GetFactory();
            using (var db = (DbConnection)dbFactory.GetConnection())
            {
                await db.OpenAsync();
                var status = await db.ExecuteScalarAsync<int>("Select 1 from IdentityUser where Email=@Email ",
                     new { Email = email });
                return status == 1 ? true : false;
            }
        }

        public async Task<IEnumerable<AppUser>> GetUsersByRoles(string role, int pageNo, int limit, string query)
        {
            var dbFactory = DbFactoryProvider.GetFactory();
            using (var db = (DbConnection)dbFactory.GetConnection())
            {
                await db.OpenAsync();
                return await db.QueryAsync<AppUser>(@"
SELECT COUNT(1) OVER() AS RowTotal, au.* FROM appuser au inner join
IdentityUserRole ir on ir.UserId=au.IdentityUserId
inner join IdentityRole r on r.Id=ir.RoleId
where  (au.FirstName like @Query or au.LastName like @Query) and r.Name=@RoleName  Order By au.AddedOn desc OFFSET @Page * (@Page-1) ROWS FETCH NEXT @Limit ROWS ONLY",
                    new { RoleName = role, Page = pageNo, Limit = limit, Query = "%" + query + "%" });

            }
        }
           public async Task<bool> CheckPhoneNumberExists(string phoneNumber)
        {
            var dbFactory = DbFactoryProvider.GetFactory();
            using (var db = (DbConnection)dbFactory.GetConnection())
            {
                await db.OpenAsync();
                var status = await db.ExecuteScalarAsync<int>("Select 1 from IdentityUser where UserName=@PhoneNumber ",
                     new { PhoneNumber = phoneNumber });
                return status == 1 ? true : false;
            }
        }



    }
}
