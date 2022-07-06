using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kachuwa.IdentityServerAdmin.Model
{
    [Table("ClientSecrets")]
    public class ClientSecrets
    {

        [Key]
        public int Id { get; set; }
        public int ClientId { get; set; }
        public string Description { get; set; }
        public string Expiration { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
    }
}