using Kachuwa.Data.Crud.Attribute;

namespace Kachuwa.Identity.Models
{
    [Table("IdentityUserRole")]
    public class IdentityUserClaim
    {
        [Key]
        public long Id { get; set; }

        public long UserId { get; set; }

        public string ClaimType { get; set; }

        public string ClaimValue { get; set; }
    }
}