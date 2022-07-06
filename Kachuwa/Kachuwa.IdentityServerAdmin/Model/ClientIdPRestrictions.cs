using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kachuwa.IdentityServerAdmin.Model
{
    [Table("ClientIdPRestrictions")]
    public class ClientIdPRestrictions
    {

        [Key]
        public int Id { get; set; }
        public int ClientId { get; set; }
        public string Provider { get; set; }
    }
}