using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using Kachuwa.Identity.Models;
using Kachuwa.Identity.Service;
using Kachuwa.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using  Microsoft.Extensions.DependencyInjection;

namespace Kachuwa.Identity.ClaimFactory
{
    //public class KachuwaClaimsPrincipalFactory<TUser, TRole> : UserClaimsPrincipalFactory<TUser,TRole>
    //    where TUser : KachuwaIdentityUser
    //    where TRole : class
    //{

    //    public KachuwaClaimsPrincipalFactory( UserManager<TUser> userManager,
    //        RoleManager<TRole> roleManager,
    //        IOptions<IdentityOptions> optionsAccessor) :base(userManager, roleManager, optionsAccessor)
    //    {

    //    }

    //    public override async Task<ClaimsPrincipal> CreateAsync(TUser user)
    //    {
    //        // return await base.CreateAsync(user);
    //        return await Task.Factory.StartNew(() =>
    //        {
    //            // appears anything works
    //            var identity = new ClaimsIdentity(new List<Claim>(), "Custom");
    //            identity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
    //            var principle = new ClaimsPrincipal(identity);

    //            return principle;
    //        });
    //    }
    //}

    public class KachuwaClaimsPrincipalFactory<TUser, TRole> : UserClaimsPrincipalFactory<TUser, TRole>
       where TUser : class
       where TRole : class
    {
        public KachuwaClaimsPrincipalFactory(UserManager<TUser> userManager, RoleManager<TRole> roleManager, IOptions<IdentityOptions> optionsAccessor) : base(userManager, roleManager, optionsAccessor)
        {
        }

        public async override Task<ClaimsPrincipal> CreateAsync(TUser user)
        {
            try
            {
                var principal = await base.CreateAsync(user);
                var identity = principal.Identities.First();

                if (!identity.HasClaim(x => x.Type == JwtClaimTypes.Subject))
                {
                    var sub = await UserManager.GetUserIdAsync(user);
                    identity.AddClaim(new Claim(JwtClaimTypes.Subject, sub));
                }

                var username = await UserManager.GetUserNameAsync(user);
                var appUserService = ContextResolver.Context.RequestServices.GetService<IAppUserService>();
                var appUser = await appUserService.AppUserCrudService.GetAsync("Where Email=@Email", new { Email = username});
                appUser = appUser ?? new AppUser();
                var usernameClaim = identity.FindFirst(claim => claim.Type == UserManager.Options.ClaimsIdentity.UserNameClaimType && claim.Value == username);
                if (usernameClaim != null)
                {
                    identity.RemoveClaim(usernameClaim);
                    identity.AddClaim(new Claim(JwtClaimTypes.PreferredUserName, username));
                    var userId = await UserManager.GetUserIdAsync(user);
                    
                    identity.AddClaim(new Claim(ApplicationClaim.IdentityUserId, userId));
                   
                    identity.AddClaim(new Claim(ApplicationClaim.AppUserId, userId));
                }

                if (!identity.HasClaim(x => x.Type == JwtClaimTypes.Name))
                {
                    identity.AddClaim(new Claim(JwtClaimTypes.Name, username));
                }
                if (!identity.HasClaim(x => x.Type == ClaimTypes.Role))
                {
                    var roles = await UserManager.GetRolesAsync(user);
                    foreach (var role in roles)
                    {
                        identity.AddClaim(new Claim(ClaimTypes.Role, role));
                    }

                }
                if (UserManager.SupportsUserEmail)
                {
                    var email = await UserManager.GetEmailAsync(user);
                    if (!String.IsNullOrWhiteSpace(email))
                    {
                        identity.AddClaims(new[]
                        {
                        new Claim(JwtClaimTypes.Email, email),
                        new Claim(JwtClaimTypes.EmailVerified,
                            await UserManager.IsEmailConfirmedAsync(user) ? "true" : "false", ClaimValueTypes.Boolean)
                    });
                    }
                }

                if (UserManager.SupportsUserPhoneNumber)
                {
                   // var phoneNumber = await UserManager.GetPhoneNumberAsync(user);
                    if (!String.IsNullOrWhiteSpace(appUser.PhoneNumber))
                    {
                        identity.AddClaims(new[]
                        {
                        new Claim(JwtClaimTypes.PhoneNumber, appUser.PhoneNumber),
                        new Claim(JwtClaimTypes.PhoneNumberVerified,
                            await UserManager.IsPhoneNumberConfirmedAsync(user) ? "true" : "false", ClaimValueTypes.Boolean)
                    });
                    }
                }

                return principal;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }



   
}