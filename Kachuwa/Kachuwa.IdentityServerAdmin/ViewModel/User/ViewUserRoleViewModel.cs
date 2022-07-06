using System.Collections.Generic;

namespace Kachuwa.IdentityServerAdmin.ViewModel
{
    public class ViewUserRoleViewModel
    {
        public List<ListRoleItemViewModel> UserRoles { get; set; }
        
        public List<ListRoleItemViewModel> AvailableRoles { get; set; }
    }
}