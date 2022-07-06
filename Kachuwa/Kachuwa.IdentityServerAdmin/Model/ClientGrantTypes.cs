﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kachuwa.IdentityServerAdmin.Model
{
    [Table("ClientGrantTypes")]
    public class ClientGrantTypes
    {

        [Key]
        public int Id { get; set; }
        public int ClientId { get; set; }
        public string GrantType { get; set; }
    }
}