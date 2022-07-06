using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kachuwa.Data.Crud.Attribute;

namespace Kachuwa.IdentityServer
{
    [Table("AspNetRoleClaims")]
    public class AspNetRoleClaims
    {
        [Key]
        public long Id { get; set; }

        public long RoleId { get; set; }

        public string ClaimType { get; set; }

        public string ClaimValue { get; set; }
    }
}