﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kachuwa.Data.Crud.Attribute;

namespace Kachuwa.Identity.Models
{
    [Table("IdentityRole")]
    public class IdentityRole: KachuwaIdentityRole
    {
        [Key]
        public new long Id { get; set; }

        // public string Name { get; set; }
        public bool IsSystem { get; set; } = false;

    }
}