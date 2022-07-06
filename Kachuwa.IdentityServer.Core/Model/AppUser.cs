using System;
using System.Collections.Generic;
using Kachuwa.Data.Crud.Attribute;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace Kachuwa.IdentityServer
{
    [Table("AppUser")]
    public class AppUser
    {
        [Key]
        public long AppUserId { get; set; }

        [IgnoreUpdate]
        public long IdentityUserId { get; set; }

        [Required(ErrorMessage = "FirstName is required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "LastName is required")]
        public string LastName { get; set; }

        public string Bio { get; set; }

        [Required(ErrorMessage = "Email address is required")]
        [IgnoreUpdate]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Username is required and must be 8 characters long")]
        [IgnoreUpdate]
        [MinLength(8, ErrorMessage = "Username must be atleast 8 characters long")]
        public string UserName { get; set; }

        public string Address { get; set; }
        [Required]
        [RegularExpression(@"^[9][6-8]{1}[0-9]{8}$")]
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "DOB  is required")]
        public string DOB { get; set; }
        public string ProfilePicture { get; set; }
        public string Gender { get; set; }

        public string CoverImage { get; set; }
        [IgnoreAll]
        public IFormFile CoverImageFile { get; set; }
        [IgnoreAll]
        public IFormFile ProfilePictureFile { get; set; }    
       
      
        public bool IsActive { get; set; }

        [AutoFill(AutoFillProperty.CurrentUserId)]
        [IgnoreUpdate]
        public int AddedBy { get; set; }
        [AutoFill(AutoFillProperty.CurrentDate)]
        [IgnoreUpdate]
        public DateTime AddedOn { get; set; }

        [AutoFill(AutoFillProperty.CurrentUserId)]
        [IgnoreInsert]
        public int UpdatedBy { get; set; }

        [AutoFill(AutoFillProperty.CurrentDate)]
        [IgnoreInsert]
        public DateTime UpdatedOn { get; set; }

        [IgnoreAll]
        public bool IsDeleted { get; set; }
        [IgnoreAll]
        public DateTime DeletedOn { get; set; }
        [AutoFill(AutoFillProperty.CurrentUserId)]
        [IgnoreInsert]
        public int DeletedBy { get; set; }

        [IgnoreAll]
        public int RowTotal { get; set; }
        [IgnoreAll]
        public string Token { get; set; }
        [IgnoreAll]
        public string RefreshToken { get; set; }
     

    }
    
    
}