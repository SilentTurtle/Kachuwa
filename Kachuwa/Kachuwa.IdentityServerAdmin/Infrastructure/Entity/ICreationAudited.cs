using System;

namespace Kachuwa.IdentityServerAdmin.Infrastructure.Entity
{
    public interface ICreationAudited
    {
        /// <summary>
        /// Creation time of this entity.
        /// </summary>
        DateTime CreationTime { get; set; }

        /// <summary>
        /// Creator of this entity.
        /// </summary>
        string CreatorUserId { get; set; }
    }
}