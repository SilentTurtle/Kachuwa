using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kachuwa.IdentityServerAdmin.Model
{
    [Table("ApiClaims")]
    public class ApiClaims
    {
        [Key]
        public int Id { get; set; }
        public int ApiResourceId { get; set; }
        public string Type { get; set; }
    }

    
}