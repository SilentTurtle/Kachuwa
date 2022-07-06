using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kachuwa.Data.Crud.Attribute;

namespace Kachuwa.IdentityServer
{
    [Table("AspNetUserRoles")]
    public class AspNetUserRoles
    {
        //[Key]
        public long UserId { get; set; }
       // [Key]
        public long RoleId { get; set; }
    }
}