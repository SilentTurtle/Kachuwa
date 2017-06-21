using System;
using Kachuwa.Data.Crud.Attribute;
using Kachuwa.Data.Crud.FormBuilder;
using System.ComponentModel.DataAnnotations;

namespace Kachuwa.Identity.Models
{
    [Table("AppUser")]
    public class AppUser
    {
        [Kachuwa.Data.Crud.Attribute.Key]
        public long AppUserId { get; set; }

        [IgnoreUpdate]
        [System.ComponentModel.DataAnnotations.Required]
        public long IdentityUserId { get; set; }

        [System.ComponentModel.DataAnnotations.Required]
        public string FirstName { get; set; }

        [System.ComponentModel.DataAnnotations.Required]
        public string LastName { get; set; }

        public string Bio { get; set; }


        [System.ComponentModel.DataAnnotations.Required]
        [IgnoreUpdate]
        public string Email { get; set; }

        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string DOB { get; set; }
        public string ProfilePicture { get; set; }

        [IgnoreUpdate]
        public bool IsActive { get; set; }

        [AutoFill(IsDate = true)]
        public DateTime AddedOn { get; set; }

        [AutoFill(GetCurrentUser = true)]
        [IgnoreUpdate]
        public string AddedBy { get; set; }

        [IgnoreAll]
        public int RowTotal { get; set; }

    }
}