using System.Collections.Generic;
using Kachuwa.Identity.Models;

namespace Kachuwa.Identity.ViewModels
{
    public class UserEditViewModel : AppUser
    {

        public List<int> UserRoleIds { get; set; }
        public List<UserRolesSelected> UserRoles { get; set; }


    }
}