using Identity.Dapper.Entities;
using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Identity.Dapper.Repositories.Contracts
{
    public interface IRoleRepository<TRole, TKey, TUserRole, TRoleClaim>
        where TRole : KachuwaIdentityRole<TKey, TUserRole, TRoleClaim>
        where TKey : IEquatable<TKey>
        where TUserRole : KachuwaIdentityUserRole<TKey>
        where TRoleClaim : KachuwaIdentityRoleClaim<TKey>
    {
        Task<bool> Insert(TRole role, CancellationToken cancellationToken, DbTransaction transaction = null);
        Task<bool> Remove(TKey id, CancellationToken cancellationToken, DbTransaction transaction = null);
        Task<bool> Update(TRole role, CancellationToken cancellationToken, DbTransaction transaction = null);
        Task<TRole> GetById(TKey id);
        Task<TRole> GetByName(string roleName);
    }
}
