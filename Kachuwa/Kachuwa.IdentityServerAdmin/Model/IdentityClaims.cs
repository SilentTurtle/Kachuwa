using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kachuwa.IdentityServerAdmin.Model
{
    [Table("IdentityClaims")]
    public class IdentityClaims
    {

        [Key]
        public int Id { get; set; }
        public int IdentityResourceId { get; set; }
        public string Type { get; set; }
    }
}