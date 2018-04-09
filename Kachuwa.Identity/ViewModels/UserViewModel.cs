using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Kachuwa.Identity.Models;

namespace Kachuwa.Identity.ViewModels
{
    public class UserViewModel : AppUser
    {
        [Required]
        public string Password { get; set; }

        public List<int> UserRoleIds { get; set; }
        public List<UserRolesSelected> UserRoles { get; set; }


    }
    public class UserRolesSelected
    {
        public long RoleId { get; set; }
        public string Name { get; set; }
        public bool IsSelected { get; set; }
    }
}