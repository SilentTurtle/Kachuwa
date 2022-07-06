using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kachuwa.Data.Crud.Attribute;

namespace Kachuwa.Identity.Models
{
    [Table("IdentityUserRole")]
    public class IdentityUserRole
    {
        //[Key]
        public long UserId { get; set; }
       // [Key]
        public long RoleId { get; set; }
    }
}