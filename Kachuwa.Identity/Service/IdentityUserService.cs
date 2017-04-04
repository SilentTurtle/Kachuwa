using Kachuwa.Data;
using Kachuwa.Identity.Models;

namespace Kachuwa.Identity.Service
{
    public class IdentityUserService: IIdentityUserService
    {
        public CrudService<IdentityUser> UserService { get; set; }=new CrudService<IdentityUser>();
        public CrudService<IdentityLogin> LoginService { get; set; }=new CrudService<IdentityLogin>();
        public CrudService<IdentityUserClaim> ClaimService { get; set; }=new CrudService<IdentityUserClaim>();
        public CrudService<IdentityUserRole> UserRoleService { get; set; }=new CrudService<IdentityUserRole>();
    }

}