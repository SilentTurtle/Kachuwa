using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kachuwa.IdentityServerAdmin.Model
{
    [Table("ClientRedirectUris")]
    public class ClientRedirectUris
    {

        [Key]
        public int Id { get; set; }
        public int ClientId { get; set; }
        public string RedirectUri { get; set; }
    }
}