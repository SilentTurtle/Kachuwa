using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IdentityServer4.Models;
using Kachuwa.Identity.Models;

namespace Kachuwa.Identity.ViewModels
{
    public class ErrorViewModel
    {
        public ErrorMessage Error { get; set; }
    }

    //public class ErrorViewModel
    //{
    //    public ErrorMessage Error { get; set; }
    //}

    //public class UserViewModel : AppUser
    //{
    //    [Required]
    //    public string Password { get; set; }

    //    public List<int> UserRoleIds { get; set; }
    //    public List<string> UserRoles { get; set; }


    //}
    //public class UserEditViewModel : AppUser
    //{

    //    public List<int> UserRoleIds { get; set; }
    //    public List<string> UserRoles { get; set; }


    //}

    //public class UserStatus
    //{
    //    public bool HasError { get; set; }
    //    public string Message { get; set; }
    //}

    //public class UserRolesViewModel
    //{
    //    public long IdentityUserId { get; set; }
    //    public List<IdentityRole> Roles { get; set; }
    //}
}