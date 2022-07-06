using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kachuwa.IdentityServerAdmin.Model
{
    [Table("ClientCorsOrigins")]
    public class ClientCorsOrigins
    {

        [Key]
        public int Id { get; set; }
        public int ClientId { get; set; }
        public string Origin { get; set; }
    }
}