﻿using System;

namespace Kachuwa.Identity.Models
{
    public class KachuwaIdentityUserRole<TKey> where TKey : IEquatable<TKey>
    {
        public virtual TKey UserId { get; set; }
        public virtual TKey RoleId { get; set; }
    }
}
