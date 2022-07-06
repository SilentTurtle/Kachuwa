using System.ComponentModel.DataAnnotations.Schema;
using Kachuwa.Data.Crud.Attribute;

namespace Kachuwa.IdentityServer
{
    [Table("AspNetUserTokens")]
    public class AspNetUserTokens
    {

        public long UserId { get; set; }

        public string LoginProvider { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }
    }
}