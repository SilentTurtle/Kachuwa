using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Validation;
using Kachuwa.Identity.Models;
using Microsoft.AspNetCore.Identity;
using IdentityUser = Kachuwa.Identity.Models.IdentityUser;
using IdentityRole = Kachuwa.Identity.Models.IdentityRole;
namespace Kachuwa.Identity.Service
{
    public class CustomResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private readonly IAppUserService _userRepository;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;


        public CustomResourceOwnerPasswordValidator(IAppUserService userRepository,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            IdentityUser user = null;
            //checking username is email
            if (context.UserName.IndexOf('@') > -1)
            {
                user = await _userManager.FindByEmailAsync(context.UserName);
            }
            else
            {
                user = await _userManager.FindByNameAsync(context.UserName);
            }

            if (user != null)
            {
                //TODO:: grab other request object and issue claim based on value
                var value = context.Request.Raw.Get("originrequest");

                if (value == "internal")
                {
                    if (context.UserName.IndexOf('@') > -1)
                    {
                        var appuser = await _userRepository.AppUserCrudService.GetAsync(
                            "Where IsActive=@isActive and Email=@email", new { email = context.UserName, isActive = true });
                        context.Result = new GrantValidationResult(appuser.IdentityUserId.ToString()
                            , OidcConstants.AuthenticationMethods.Password, new List<Claim>()
                            {
                                new Claim("use", "internal")
                            });
                    }
                    else
                    {
                        var appuser = await _userRepository.AppUserCrudService.GetAsync(
                            "Where IsActive=@isActive and UserName=@UserName", new { UserName = context.UserName, isActive = true });
                        context.Result = new GrantValidationResult(appuser.IdentityUserId.ToString()
                            , OidcConstants.AuthenticationMethods.Password, new List<Claim>()
                            {
                                new Claim("use", "internal")
                            });
                    }

                  

                }
                else
                {
                    var result = await _signInManager.CheckPasswordSignInAsync(user, context.Password, true);
                    if (result.Succeeded)
                    {


                        AppUser appuser = null;

                        if (context.UserName.IndexOf('@') > -1)
                        {


                            appuser=await _userRepository.AppUserCrudService.GetAsync(
                                "Where IsActive=@isActive and Email=@email",
                                new {email = context.UserName, isActive = true});
                        }
                        else
                        {
                            appuser = await _userRepository.AppUserCrudService.GetAsync(
                             "Where IsActive=@isActive and UserName=@UserName",
                             new { UserName = context.UserName, isActive = true });
                        }

                        if (!string.IsNullOrEmpty(value))
                        {
                            if (value == "pos")
                            {
                                context.Result = new GrantValidationResult(appuser.IdentityUserId.ToString()
                                    , OidcConstants.AuthenticationMethods.Password, new List<Claim>()
                                    {
                                                                new Claim("sc", "pos")
                                    });
                            }
                            else if (value == "apps")
                            {
                                context.Result = new GrantValidationResult(appuser.IdentityUserId.ToString()
                                    , OidcConstants.AuthenticationMethods.Password, new List<Claim>()
                                    {
                                                                new Claim("sc", "mobile")
                                    });
                            }
                            else
                            {
                                context.Result = new GrantValidationResult(appuser.IdentityUserId.ToString()
                                    , OidcConstants.AuthenticationMethods.Password, new List<Claim>()
                                    {
                                                                new Claim("sc", "web")
                                    });
                            }

                        }
                        else
                        {
                            context.Result = new GrantValidationResult(appuser.IdentityUserId.ToString()
                                , OidcConstants.AuthenticationMethods.Password);
                        }

                    }

                    //if (_userRepository.ValidateCredentials(context.UserName, context.Password))
                    //{
                    //    var user = _userRepository.FindByUsername(context.UserName);
                    //    context.Result = new GrantValidationResult(user.SubjectId, OidcConstants.AuthenticationMethods.Password);
                    //}
                }
            }

            await Task.FromResult(0);
        }
    }
}