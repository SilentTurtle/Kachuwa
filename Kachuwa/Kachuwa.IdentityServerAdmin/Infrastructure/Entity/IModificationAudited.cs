using System;

namespace Kachuwa.IdentityServerAdmin.Infrastructure.Entity
{
    public interface IModificationAudited
    {
        /// <summary>
        /// Last modification date of this entity.
        /// </summary>
        DateTime? LastModificationTime { get; set; }

        /// <summary>
        /// Last modifier user of this entity.
        /// </summary>
        string LastModifierUserId { get; set; }
    }
}