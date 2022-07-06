using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kachuwa.Data.Crud.Attribute;
using Microsoft.AspNetCore.Http;

namespace Kachuwa.IdentityServer.ViewModel
{
    public class UserRolesViewModel
    {
        public long IdentityUserId { get; set; }
        public List<KachuwaIdentityRole> Roles { get; set; }
    }
    public class UserEditViewModel : AppUser
    {

        public List<int> UserRoleIds { get; set; }
        public List<UserRolesSelected> UserRoles { get; set; }


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
    public class UserStatus
    {
        public bool HasError { get; set; }
        public string Message { get; set; }
        public long IdentityUserId { get; set; }
    }
    public class UserDeviceStatus
    {
        public bool MobileDevice { get; set; }
        public int VerifiedDeviceCount { get; set; }
        public bool IsThisUnverifiedLogin { get; set; }
        public int BrowserCount { get; set; }
        public int MobileCount { get; set; }
    }

    public class UserViewModel : AppUser
    {
        [Required]
        public string Password { get; set; }

        public List<UserRolesSelected> UserRoles { get; set; }
        [IgnoreAll]
        public IFormFile ImageFile { get; set; }

        [IgnoreAll]
        public string ImportMessage { get; set; }

    }
    public class UserRolesSelected
    {
        public long RoleId { get; set; }
        public string Name { get; set; }
        public bool IsSelected { get; set; }
    }
    public class DeviceVerificationStatus
    {
        public bool IsVerified { get; set; }
        public string Message { get; set; }
    }
    public class DeviceRemovalViewModel
    {
        [Required]
        public long UserDeviceId { get; set; }
    }
}
