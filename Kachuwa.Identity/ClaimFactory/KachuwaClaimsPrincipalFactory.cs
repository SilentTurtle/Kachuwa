using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Kachuwa.Identity.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Kachuwa.Identity.ClaimFactory
{
    public class KachuwaClaimsPrincipalFactory<TUser, TRole> : UserClaimsPrincipalFactory<TUser,TRole>
        where TUser : KachuwaIdentityUser
        where TRole : class
    {

        public KachuwaClaimsPrincipalFactory( UserManager<TUser> userManager,
            RoleManager<TRole> roleManager,
            IOptions<IdentityOptions> optionsAccessor) :base(userManager, roleManager, optionsAccessor)
        {
                
        }

        public override async Task<ClaimsPrincipal> CreateAsync(TUser user)
        {
            // return await base.CreateAsync(user);
            return await Task.Factory.StartNew(() =>
            {
                // appears anything works
                var identity = new ClaimsIdentity(new List<Claim>(), "Custom");
                identity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
                var principle = new ClaimsPrincipal(identity);

                return principle;
            });
        }
    }
}