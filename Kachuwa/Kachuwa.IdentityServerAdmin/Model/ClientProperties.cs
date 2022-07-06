using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kachuwa.IdentityServerAdmin.Model
{
    [Table("ClientProperties")]
    public class ClientProperties
    {

        [Key]
        public int Id { get; set; }
        public int ClientId { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }
}