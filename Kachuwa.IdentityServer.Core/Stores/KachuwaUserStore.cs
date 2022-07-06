﻿using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Data.Common;
using System.Security.Claims;
using Kachuwa.Log;
using IdentityUser = Kachuwa.IdentityServer.AspNetUsers;
using IdentityRole = Kachuwa.IdentityServer.AspNetRoles;
using IdentityLogin = Kachuwa.IdentityServer.AspNetUserLogins;
using IdentityUserClaim = Kachuwa.IdentityServer.AspNetUserClaims;
using IdentityUserRole = Kachuwa.IdentityServer.AspNetUserRoles;
using Kachuwa.Data;
using Kachuwa.Data.Crud;
using Kachuwa.IdentityServer.Service;

namespace Kachuwa.IdentityServer.Stores
{
    public class KachuwaUserStore<TUser, TKey, TUserRole, TRoleClaim, TUserClaim, TUserLogin, TRole> :
        IUserStore<TUser>,
        IUserLoginStore<TUser>,
        IUserRoleStore<TUser>,
        IUserClaimStore<TUser>,
        IUserPasswordStore<TUser>,
        IUserSecurityStampStore<TUser>,
        IUserEmailStore<TUser>,
        IUserLockoutStore<TUser>,
        IUserPhoneNumberStore<TUser>,
        IQueryableUserStore<TUser>,
        IUserTwoFactorStore<TUser>,
        IUserAuthenticationTokenStore<TUser>
        where TUser : KachuwaIdentityUser<TKey, TUserClaim, TUserRole, TUserLogin>
        where TKey : IEquatable<TKey>
        where TUserRole : KachuwaIdentityUserRole<TKey>
        where TRoleClaim : KachuwaIdentityRoleClaim<TKey>
        where TUserClaim : KachuwaIdentityUserClaim<TKey>
        where TUserLogin : KachuwaIdentityUserLogin<TKey>
        where TRole : KachuwaIdentityRole<TKey, TUserRole, TRoleClaim>
    {
        private DbTransaction _transaction;
        private DbConnection _connection;


        private readonly ILogger<KachuwaUserStore<TUser, TKey, TUserRole, TRoleClaim, TUserClaim, TUserLogin, TRole>> _log;
        private readonly Kachuwa.Log.ILogger _logger;

        private readonly IIdentityUserService _userService;
        private readonly IIdentityRoleService _roleService;
        public KachuwaUserStore(IIdentityUserService userService,
                                IIdentityRoleService roleService,
                               ILogger<KachuwaUserStore<TUser, TKey, TUserRole, TRoleClaim, TUserClaim, TUserLogin, TRole>> log, Kachuwa.Log.ILogger logger
                              )
        {

            _userService = userService;
            _roleService = roleService;
            _log = log;
            _logger = logger;


        }


        public IQueryable<TUser> Users
        {
            get
            {
                //Impossible to implement IQueryable with Dapper
                throw new NotImplementedException();
            }
        }

        public async Task AddClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            try
            {

                foreach (var claim in claims)
                {
                    await _userService.ClaimService.InsertAsync<int>(claim);
                }
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message, ex);
            }
        }

        public async Task AddLoginAsync(TUser user, UserLoginInfo login, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            try
            {
                var model = new IdentityLogin()
                {
                    UserId = long.Parse(user.Id.ToString()),
                    LoginProvider = login.LoginProvider,
                    ProviderDisplayName = login.ProviderDisplayName,
                    ProviderKey = login.ProviderKey
                };
                await _userService.LoginService.InsertAsync<int>(model);
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message, ex);
                _logger.Log(LogType.Error, () => ex.Message, ex);
            }
        }

        public async Task AddToRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            try
            {
                var role = await _roleService.RoleService.GetAsync("Where Name=@Name", new { Name = roleName });

                await _userService.UserRoleService.InsertAsync<int>(new IdentityUserRole
                {
                    UserId = long.Parse(user.Id.ToString()),
                    RoleId = role.Id
                });

            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message, ex);
                _logger.Log(LogType.Error, () => ex.Message, ex);
            }
        }

        public async Task<IdentityResult> CreateAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            try
            {


                var model = user as IdentityUser;
                var result = await _userService.UserService.InsertAsync<TKey>(model);


                if (!result.Equals(default(TKey)))
                {
                    user.Id = result;
                    return IdentityResult.Success;
                }
                else
                {
                    return IdentityResult.Failed();
                }
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message, ex);
                _logger.Log(LogType.Error, () => ex.Message, ex);

                return IdentityResult.Failed();
            }
        }

        public async Task<IdentityResult> DeleteAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            try
            {
                await _userService.UserRoleService.DeleteAsync("Where UserId=@Id", new { Id = user.Id });

                var result = await _userService.UserService.DeleteAsync(user.Id);
                return result > 0 ? IdentityResult.Success : IdentityResult.Failed();

            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message, ex);
                _logger.Log(LogType.Error, () => ex.Message, ex);

                return IdentityResult.Failed();
            }
        }

        public void Dispose()
        {
        }

        public async Task<TUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrEmpty(normalizedEmail))
                throw new ArgumentNullException(nameof(normalizedEmail));

            try
            {
                //var _appUserService = ContextResolver.Context.RequestServices.GetService<IAppUserService>();
                //var appuser = await _appUserService.AppUserCrudService.GetAsync(
                //    "Where Email=@Email and IsActive=@IsActive", new {Email = normalizedEmail, IsActive = true});
                //if (appuser == null)
                //    return null;
                var result = await _userService.UserService.GetAsync("Where Email=@Email", new { Email = normalizedEmail });
                return result as TUser;
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message, ex);
                _logger.Log(LogType.Error, () => ex.Message, ex);

                return null;
            }
        }

        public async Task<TUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrEmpty(userId))
                throw new ArgumentNullException(nameof(userId));

            try
            {
                //var _appUserService = ContextResolver.Context.RequestServices.GetService<IAppUserService>();
                //var appuser = await _appUserService.AppUserCrudService.GetAsync(
                //    "Where IdentityUserId=@IdentityUserId and IsActive=@IsActive", new { IdentityUserId = userId, IsActive = true });
                //if (appuser == null)
                //    return null;
                var result = await _userService.UserService.GetAsync("Where Id=@Id", new { Id = userId });

                return result as TUser;
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message, ex);
                _logger.Log(LogType.Error, () => ex.Message, ex);

                return null;
            }
        }

        public async Task<TUser> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                var loginifo = await _userService.GetExternalUserAsync(loginProvider, providerKey);
                var result = await _userService.UserService.GetAsync(loginifo.UserId);
                return result as TUser;
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message, ex);

                return null;
            }
        }

        public async Task<TUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrEmpty(normalizedUserName))
                throw new ArgumentNullException(nameof(normalizedUserName));

            try
            {

                if (normalizedUserName.IndexOf('@') > -1)
                {
                    //var _appUserService = ContextResolver.Context.RequestServices.GetService<IAppUserService>();
                    //var appuser = await _appUserService.AppUserCrudService.GetAsync(
                    //    "Where Email=@Email and IsActive=@IsActive", new { Email = normalizedUserName, IsActive = true });
                    //if (appuser == null)
                    //    return null;
                    var result = await _userService.UserService.GetAsync("Where Email=@Email", new { Email = normalizedUserName });

                    return result as TUser;
                }
                else
                {
                    //var _appUserService = ContextResolver.Context.RequestServices.GetService<IAppUserService>();
                    //var appuser = await _appUserService.AppUserCrudService.GetAsync(
                    //    "Where UserName=@UserName and IsActive=@IsActive", new { UserName = normalizedUserName, IsActive = true });
                    //if (appuser == null)
                    //    return null;
                    var result = await _userService.UserService.GetAsync("Where UserName=@UserName", new { UserName = normalizedUserName });

                    return result as TUser;
                }

            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message, ex);

                return null;
            }
        }

        public Task<int> GetAccessFailedCountAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.AccessFailedCount);
        }

        public async Task<IList<Claim>> GetClaimsAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            try
            {
                var result = await _userService.ClaimService.GetListAsync("Where UserId=@UserId", new { UserId = user.Id });

                return result.Select(e => new Claim(e.ClaimType, e.ClaimType)).ToList();

            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message, ex);

                return null;
            }
        }

        public Task<string> GetEmailAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.EmailConfirmed);
        }

        public Task<bool> GetLockoutEnabledAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.LockoutEnabled);
        }

        public Task<DateTimeOffset?> GetLockoutEndDateAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.LockoutEnd);
        }

        public async Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            try
            {
                var loginifo = await _userService.LoginService.GetListAsync("Where UserId=@UserId", new { UserId = user.Id });

                return loginifo.Select(e => new UserLoginInfo(e.LoginProvider, e.ProviderKey, e.ProviderDisplayName)).ToList();

            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message, ex);

                return null;
            }
        }

        public Task<string> GetNormalizedEmailAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.Email);
        }

        public Task<string> GetNormalizedUserNameAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.UserName);
        }

        public Task<string> GetPasswordHashAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.PasswordHash);
        }

        public Task<string> GetPhoneNumberAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.PhoneNumber);
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.PhoneNumberConfirmed);
        }

        public async Task<IList<string>> GetRolesAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            try
            {
                var userRoles = await _userService.UserRoleService.GetListAsync("Where UserId=@UserId",
                    new { UserId = user.Id });
                var roles = DbFactoryProvider.GetFactory().Dialect == Dialect.PostgreSQL ? await _roleService.RoleService.GetListAsync("Where Id=any(@RoleIds) ",
                    new { RoleIds = userRoles.Select(e => e.RoleId).ToArray() }):
                     await _roleService.RoleService.GetListAsync("Where Id in @RoleIds",
                    new { RoleIds = userRoles.Select(e => e.RoleId).ToArray() });

                return roles.Select(x => x.Name).ToList();
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message, ex);

                return null;
            }
        }

        public Task<string> GetSecurityStampAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.SecurityStamp);
        }

        public Task<string> GetTokenAsync(TUser user, string loginProvider, string name, CancellationToken cancellationToken)
        {
            return Task.FromResult(string.Empty);
        }

        public Task<bool> GetTwoFactorEnabledAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.TwoFactorEnabled);
        }

        public Task<string> GetUserIdAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.Id.ToString());
        }

        public Task<string> GetUserNameAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.UserName);
        }

        public async Task<IList<TUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (claim == null)
                throw new ArgumentNullException(nameof(claim));

            try
            {
                var usersid = await _userService.ClaimService.GetListAsync("Where ClaimValue=@cv and ClaimType=@ct",
                    new { ct = claim.Type, cv = claim.Value });
                var users = DbFactoryProvider.GetFactory().Dialect == Dialect.PostgreSQL ? await _userService.UserService.GetListAsync("Where UserId=any(@uids)",
                      new { uids = usersid.Select(e => e.UserId).ToArray() })
                    : await _userService.UserService.GetListAsync("Where UserId in @uids",
                      new { uids = usersid.Select(e => e.UserId).ToArray() });

                return users as IList<TUser>;
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message, ex);

                return null;
            }
        }

        public async Task<IList<TUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrEmpty(roleName))
                throw new ArgumentNullException(nameof(roleName));

            try
            {

                var role = await _roleService.RoleService.GetAsync("Where Name=@Name",
                    new { Name = roleName });
                var userinRoles = await _userService.UserRoleService.GetListAsync("Where RoleId=@RoleId",
                   new { RoleId = role.Id });
                var users = DbFactoryProvider.GetFactory().Dialect == Dialect.PostgreSQL ? await _userService.UserService.GetListAsync("Where Id=any(@UserIds)",
                new { UserIds = userinRoles.Select(e => e.UserId).ToArray() }) :
                await _userService.UserService.GetListAsync("Where Id in @UserIds",
                new { UserIds = userinRoles.Select(e => e.UserId).ToArray() })
                ;

                return users as IList<TUser>;
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message, ex);

                return null;
            }
        }

        public Task<bool> HasPasswordAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(user.PasswordHash != null);
        }

        public Task<int> IncrementAccessFailedCountAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            user.AccessFailedCount++;
            return Task.FromResult(user.AccessFailedCount);
        }

        public async Task<bool> IsInRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrEmpty(roleName))
                throw new ArgumentNullException(nameof(roleName));

            try
            {
                var role = await _roleService.RoleService.GetAsync("Where Name=@Name",
                   new { Name = roleName });
                var userinfo = await _userService.UserRoleService.GetAsync("Where UserId=@UserId and RoleId=@RoleId",
                   new { UserId = user.Id, RoleId = role.Id });

                return userinfo != null;
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message, ex);

                return false;
            }
        }

        public async Task RemoveClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (claims == null)
                throw new ArgumentNullException(nameof(claims));

            try
            {
                //TODO::here delete claims with 
                foreach (var claim in claims)
                {
                    await _userService.ClaimService.DeleteAsync(
                        "Where UserId=@UserId and ClaimType=@ct and ClaimValue=@cv",
                        new
                        {
                            UserId = user.Id,
                            cv = claim.Value,
                            ct = claim.Type
                        });

                }

            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message, ex);
            }
        }

        public async Task RemoveFromRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrEmpty(roleName))
                throw new ArgumentNullException(nameof(roleName));

            try
            {
                var role = await _roleService.RoleService.GetAsync("Where Name=@Name",
                   new { Name = roleName });
                await _userService.UserRoleService.DeleteAsync("Where UserId=@UserId and RoleId=@RoleId", new { UserId = user.Id, RoleId = role.Id });
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message, ex);
            }
        }

        public async Task RemoveLoginAsync(TUser user, string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrEmpty(loginProvider))
                throw new ArgumentNullException(nameof(loginProvider));

            if (string.IsNullOrEmpty(providerKey))
                throw new ArgumentNullException(nameof(providerKey));

            try
            {

                // await _userService.LoginService.DeleteAsync("Where UserId=@UserId and LoginProvider=@lp and ProviderKey=@pk", new { UserId = user.Id, lp = loginProvider, pk = providerKey });

                var id = user.Id.ToString();
                await _userService.DeleteUserLoginProvider(Convert.ToInt64(id), loginProvider);
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message, ex);
            }
        }

        public Task RemoveTokenAsync(TUser user, string loginProvider, string name, CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }

        public async Task ReplaceClaimAsync(TUser user, Claim claim, Claim newClaim, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();


            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (claim == null)
                throw new ArgumentNullException(nameof(claim));

            if (newClaim == null)
                throw new ArgumentNullException(nameof(newClaim));

            try
            {
                var model = new IdentityUserClaim();
                model.UserId = long.Parse(user.Id.ToString());
                model.ClaimType = claim.Type;
                model.ClaimValue = claim.Value;
                await _userService.ClaimService.UpdateAsync(model,
                    "Where UserId=@UserId and ClaimType=@ct and ClaimValue=@cv"
                    , new
                    {
                        UserId = user.Id,
                        ct = claim.Type,
                        cv = claim.Value
                    });

            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message, ex);
            }
        }

        public Task ResetAccessFailedCountAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            user.AccessFailedCount = 0;

            return Task.FromResult(0);
        }

        public Task SetEmailAsync(TUser user, string email, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            user.Email = email;

            return Task.FromResult(0);
        }

        public Task SetEmailConfirmedAsync(TUser user, bool confirmed, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            user.EmailConfirmed = confirmed;

            return Task.FromResult(0);
        }

        public Task SetLockoutEnabledAsync(TUser user, bool enabled, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            user.LockoutEnabled = enabled;

            return Task.FromResult(0);
        }

        public Task SetLockoutEndDateAsync(TUser user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            user.LockoutEnd = lockoutEnd;

            return Task.FromResult(0);
        }

        public Task SetNormalizedEmailAsync(TUser user, string normalizedEmail, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            user.Email = normalizedEmail;

            return Task.FromResult(0);
        }

        public Task SetNormalizedUserNameAsync(TUser user, string normalizedName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(0);
        }

        public Task SetPasswordHashAsync(TUser user, string passwordHash, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            user.PasswordHash = passwordHash;

            return Task.FromResult(0);
        }

        public Task SetPhoneNumberAsync(TUser user, string phoneNumber, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            user.PhoneNumber = phoneNumber;

            return Task.FromResult(0);
        }

        public Task SetPhoneNumberConfirmedAsync(TUser user, bool confirmed, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            user.PhoneNumberConfirmed = confirmed;

            return Task.FromResult(0);
        }

        public Task SetSecurityStampAsync(TUser user, string stamp, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            user.SecurityStamp = stamp;

            return Task.FromResult(0);
        }

        public Task SetTokenAsync(TUser user, string loginProvider, string name, string value, CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }

        public Task SetTwoFactorEnabledAsync(TUser user, bool enabled, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            user.TwoFactorEnabled = enabled;

            return Task.FromResult(0);
        }

        public Task SetUserNameAsync(TUser user, string userName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            user.UserName = userName;

            return Task.FromResult(0);
        }

        public async Task<IdentityResult> UpdateAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            // await CreateTransactionIfNotExists(cancellationToken);

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            try
            {
                var status = await _userService.UserService.UpdateAsync(user);

                return status ? IdentityResult.Success : IdentityResult.Failed();
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message, ex);

                return IdentityResult.Failed();
            }
        }

    }
}
