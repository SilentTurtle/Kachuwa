using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kachuwa.IdentityServerAdmin.Model
{
    [Table("ApiScopeClaims")]
    public class ApiScopeClaims
    {

        [Key]
        public int Id { get; set; }
        public int ApiScopeId { get; set; }
        public string Type { get; set; }
    }
}