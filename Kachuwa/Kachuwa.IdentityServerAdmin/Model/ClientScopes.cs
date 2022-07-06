using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kachuwa.IdentityServerAdmin.Model
{
    [Table("ClientScopes")]
    public class ClientScopes
    {

        [Key]
        public int Id { get; set; }
        public int ClientId { get; set; }
        public string Scope { get; set; }
    }
}