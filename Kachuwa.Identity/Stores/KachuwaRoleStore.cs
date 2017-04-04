using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Kachuwa.Identity.Models;
using Kachuwa.Identity.Service;

namespace Kachuwa.Identity.Stores
{
    public class KachuwaRoleStore<TRole, TKey, TUserRole, TRoleClaim>
        : IRoleStore<TRole>
        where TRole : KachuwaIdentityRole<TKey, TUserRole, TRoleClaim>
        where TKey : IEquatable<TKey>
        where TUserRole : KachuwaIdentityUserRole<TKey>
        where TRoleClaim : KachuwaIdentityRoleClaim<TKey>
    {
        private readonly ILogger<KachuwaRoleStore<TRole, TKey, TUserRole, TRoleClaim>> _log;
        private readonly IIdentityRoleService _roleService;
        public KachuwaRoleStore(IIdentityRoleService roleService,
                               ILogger<KachuwaRoleStore<TRole, TKey, TUserRole, TRoleClaim>> log

                               )
        {
            _roleService = roleService;
            _log = log;
        }

        public async Task<IdentityResult> CreateAsync(TRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (role == null)
                throw new ArgumentNullException(nameof(role));

            try
            {
                var result = await _roleService.RoleService.InsertAsync<int>(role as KachuwaIdentityRole);
                return result > 0 ? IdentityResult.Success : IdentityResult.Failed();
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message, ex);

                return IdentityResult.Failed();
            }
        }

        public async Task<IdentityResult> DeleteAsync(TRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (role == null)
                throw new ArgumentNullException(nameof(role));

            try
            {
                var result = await _roleService.RoleService.DeleteAsync(role.Id);
                return result > 0 ? IdentityResult.Success : IdentityResult.Failed();
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message, ex);

                return IdentityResult.Failed();
            }
        }

        public void Dispose()
        {

        }

        public async Task<TRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrEmpty(roleId))
                throw new ArgumentNullException(nameof(roleId));

            try
            {
                var result = await _roleService.RoleService.GetAsync(roleId);
                return result as TRole;
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message, ex);

                return null;
            }
        }

        public async Task<TRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrEmpty(normalizedRoleName))
                throw new ArgumentNullException(nameof(normalizedRoleName));

            try
            {
                var result = await _roleService.RoleService.GetAsync("Where Name=@Name", new { Name = normalizedRoleName });
                return result as TRole;
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message, ex);

                return null;
            }
        }

        public Task<string> GetNormalizedRoleNameAsync(TRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (role == null)
                throw new ArgumentNullException(nameof(role));

            return Task.FromResult(role.Name);
        }

        public Task<string> GetRoleIdAsync(TRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (role == null)
                throw new ArgumentNullException(nameof(role));

            if (role.Id.Equals(default(TKey)))
                return null;

            return Task.FromResult(role.Id.ToString());
        }

        public Task<string> GetRoleNameAsync(TRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (role == null)
                throw new ArgumentNullException(nameof(role));

            return Task.FromResult(role.Name);
        }

        public Task SetNormalizedRoleNameAsync(TRole role, string normalizedName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (role == null)
                throw new ArgumentNullException(nameof(role));

            return Task.FromResult(0);
        }

        public Task SetRoleNameAsync(TRole role, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (role == null)
                throw new ArgumentNullException(nameof(role));

            role.Name = roleName;

            return Task.FromResult(0);
        }

        public async Task<IdentityResult> UpdateAsync(TRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (role == null)
                throw new ArgumentNullException(nameof(role));

            try
            {
                var result = await _roleService.RoleService.UpdateAsync(role);
                return result ? IdentityResult.Success : IdentityResult.Failed();
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message, ex);

                return IdentityResult.Failed();
            }
        }
    }
}
