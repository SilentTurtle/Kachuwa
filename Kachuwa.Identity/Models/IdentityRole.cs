using Kachuwa.Data.Crud.Attribute;

namespace Kachuwa.Identity.Models
{
    [Table("IdentityTole")]
    public class IdentityRole: KachuwaIdentityRole
    {
        [Key]
        public new int Id { get; set; }

        // public string Name { get; set; }
    }
}