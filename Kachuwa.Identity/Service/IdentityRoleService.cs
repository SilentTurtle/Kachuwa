using Kachuwa.Data;
using Kachuwa.Identity.Models;

namespace Kachuwa.Identity.Service
{
    public class IdentityRoleService : IIdentityRoleService
    {
        public CrudService<IdentityRole> RoleService { get; set; } = new CrudService<IdentityRole>();
    }


}