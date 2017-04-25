using Kachuwa.Data.Crud.Attribute;

namespace Kachuwa.Identity.Models
{
    [Table("IdentityUserLogin")]
    public class IdentityUserLogin
    {
        public long UserId { get; set; }

        public string ProviderKey { get; set; }

        public string LoginProvider { get; set; }

        public string ProviderDisplayName { get; set; }
    }
}