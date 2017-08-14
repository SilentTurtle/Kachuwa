using System.ComponentModel.DataAnnotations.Schema;
using Kachuwa.Data.Crud.Attribute;

namespace Kachuwa.Identity.Models
{
    [Table("IdentityUser")]
    public class IdentityUser:KachuwaIdentityUser
    {
        
        //public new long Id { get; set; }

        //public string Username { get; set; }

        //public string Email { get; set; }

        //public bool EmailConfirmed { get; set; }

        //public string PasswordHash { get; set; }

        //public string SecurityStamp { get; set; }

        //public string PhoneNumber { get; set; }

        //public bool PhoneNumberConfirmed { get; set; }

        //public bool TwoFactorEnabled { get; set; }

        //public DateTime LockoutEnd { get; set; }

        //public bool LockoutEnabled { get; set; }

        //public int AccessFailedCount { get; set; }
    }
}