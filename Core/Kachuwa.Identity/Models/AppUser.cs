using System;
using System.Collections.Generic;
using Kachuwa.Data.Crud.Attribute;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kachuwa.Identity.ViewModels;
using Microsoft.AspNetCore.Http;

namespace Kachuwa.Identity.Models
{
    [Table("AppUser")]
    public class AppUser
    {
        [Key]
        public long AppUserId { get; set; }

        [IgnoreUpdate]
        public long IdentityUserId { get; set; }

        [Required(ErrorMessage = "User.FirstName.Required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "User.LastName.Required")]
        public string LastName { get; set; }

        public string Bio { get; set; }

        [Required(ErrorMessage = "User.Email.Required")]
        [IgnoreUpdate]
        [EmailAddress(ErrorMessage = "User.InvalidEmail")]
        public string Email { get; set; }

        [Required(ErrorMessage = "User.UserName.Required")]
        [IgnoreUpdate]
        [MinLength(8, ErrorMessage = "User.UserName.MinLength")]
        public string UserName { get; set; }

        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string DOB { get; set; }
        public string ProfilePicture { get; set; }
        public string Gender { get; set; }             
       
       // public string CoverImage { get; set; }
        [IgnoreAll]
        public IFormFile CoverImageFile { get; set; }
        
        public bool IsActive { get; set; }
        [IgnoreInsert]
        [AutoFill(false)]
        public bool IsDeleted { get; set; }
        [AutoFill(AutoFillProperty.CurrentDate)]
        [IgnoreUpdate]
        public DateTime AddedOn { get; set; }

        [AutoFill(AutoFillProperty.CurrentUser)]
        [IgnoreUpdate]
        public string AddedBy { get; set; }

        [IgnoreAll]
        public int RowTotal { get; set; }
        [IgnoreAll]
        public string Token { get; set; }
        [IgnoreAll]
        public string RefreshToken { get; set; }
        public string GroupName { get; set; }
    }

    public class AppUserRegisterModel : AppUser
    {
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
    public class ProfilePictureViewModel
    {
        public long IdentityUserId { get; set; }
        public string ImagePath { get; set; }

    }
    public class RefreshTokenViewModel
    {
        [Required]
        public string RefreshToken { get; set; }

    }

    public class UserPrintouViewModel : AppUser
    {
        public string PackageNames { get; set; }
        public string Organization { get; set; }
    }

    public class UserExportViewModel
    {
       
        public string FirstName { get; set; }

      
        public string LastName { get; set; }
      
        public string Email { get; set; }
      
        public string UserName { get; set; }
     
        public string PhoneNumber { get; set; }
        public string GroupName { get; set; }
        //public int OrganizationId { get; set; }
        //public string Organization { get; set; }

    }

    public class UserImportViewModel
    {
        public IFormFile ImportFile { get; set; }      
        public List<UserRolesSelected> UserRoles { get; set; }
        public bool AutoGenerateEmailAddress { get; set; }
        public bool AutoGenerateUserName { get; set; }
        public List<UserViewModel> Users { get; set; }=new List<UserViewModel>();

        public bool ImportStatus { get; set; }
        public string Message { get; set; }
    }
}