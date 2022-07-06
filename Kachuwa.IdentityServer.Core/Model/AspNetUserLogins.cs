using System.ComponentModel.DataAnnotations.Schema;
using Kachuwa.Data.Crud.Attribute;

namespace Kachuwa.IdentityServer
{
    [Table("AspNetUserLogins")]
    public class AspNetUserLogins
    {

        public string LoginProvider { get; set; }

        public string ProviderKey { get; set; }

        public long UserId { get; set; }

        public string ProviderDisplayName { get; set; }

      
    }
}