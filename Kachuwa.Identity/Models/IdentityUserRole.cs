﻿using Kachuwa.Data.Crud.Attribute;

namespace Kachuwa.Identity.Models
{
    [Table("IdentityUserRole")]
    public class IdentityUserRole
    {

        public long UserId { get; set; }

        public int RoleId { get; set; }
    }
}