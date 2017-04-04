using Kachuwa.Data;
using Kachuwa.Identity.Models;

namespace Kachuwa.Identity.Service
{
    public interface IIdentityUserService
    {
        CrudService<IdentityUser> UserService { get; set; }
        CrudService<IdentityLogin> LoginService { get; set; }
        CrudService<IdentityUserClaim> ClaimService { get; set; }
        CrudService<IdentityUserRole> UserRoleService { get; set; }
    }
}