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



    [Table("UserType")]
    public class UserType
    {
        [Key]
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        [IgnoreAll]
        public bool IsDeleted { get; set; }
        [AutoFill(AutoFillProperty.CurrentDate)]
        [IgnoreUpdate]
        public DateTime AddedOn { get; set; }
        [AutoFill(AutoFillProperty.CurrentUserId)]
        [IgnoreUpdate]
        public long AddedBy { get; set; }
        [IgnoreInsert]
        [AutoFill(AutoFillProperty.CurrentUserId)]
        public long DeletedBy { get; set; }
        [IgnoreInsert]
        [AutoFill(AutoFillProperty.CurrentDate)]
        public DateTime DeletedOn { get; set; }
        [IgnoreInsert]
        [AutoFill(AutoFillProperty.CurrentDate)]
        public DateTime UpdatedOn { get; set; }
        [AutoFill(AutoFillProperty.CurrentUserId)]
        [IgnoreInsert]
        public long UpdatedBy { get; set; }
        [IgnoreAll]
        public int RowTotal { get; set; }
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
    }

    public class UserImportViewModel
    {
        public IFormFile ImportFile { get; set; }
        public List<UserRolesSelected> UserRoles { get; set; }
        public bool AutoGenerateEmailAddress { get; set; }
        public bool AutoGenerateUserName { get; set; }
        public List<UserViewModel> Users { get; set; } = new List<UserViewModel>();

        public bool ImportStatus { get; set; }
        public string Message { get; set; }
    }
}