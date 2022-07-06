using System.Collections.Generic;
using Kachuwa.Identity.Models;

namespace Kachuwa.Identity.ViewModels
{
    public class UserRolesViewModel
    {
        public long IdentityUserId { get; set; }
        public List<IdentityRole> Roles { get; set; }
    }
}