using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Kachuwa.IdentityServer
{
    public class KachuwaIdentityRole : KachuwaIdentityRole<int>
    {
        public KachuwaIdentityRole() { }
        public KachuwaIdentityRole(string roleName) : this()
        {
            Name = roleName;
        }
    }

    public class KachuwaIdentityRole<TKey> : KachuwaIdentityRole<TKey, KachuwaIdentityUserRole<TKey>, KachuwaIdentityRoleClaim<TKey>>
        where TKey : IEquatable<TKey>
    {
        public KachuwaIdentityRole() { }
        public KachuwaIdentityRole(string roleName) : this()
        {
            Name = roleName;
        }
    }

    public class KachuwaIdentityRole<TKey, TUserRole, TRoleClaim>
        where TKey : IEquatable<TKey>
        where TUserRole : KachuwaIdentityUserRole<TKey>
        where TRoleClaim : KachuwaIdentityRoleClaim<TKey>
    {
        #region Properties 

        public virtual ICollection<TUserRole> Users { get; } = new List<TUserRole>();
        public virtual ICollection<TRoleClaim> Claims { get; } = new List<TRoleClaim>();
        public virtual TKey Id { get; set; }
        [Required(ErrorMessage ="Role.Name.Required")]
        public virtual string Name { get; set; }

       /// public virtual bool IsSystem { get; set; } 
        #endregion

        #region Constructors

        public KachuwaIdentityRole() { }

        public KachuwaIdentityRole(string roleName) : this()
        {
            Name = roleName;
        }

        #endregion
    }
}
