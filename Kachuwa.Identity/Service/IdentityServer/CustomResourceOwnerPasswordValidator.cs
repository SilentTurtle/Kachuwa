using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Validation;
using Kachuwa.Identity.Models;
using Microsoft.AspNetCore.Identity;

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
            var result = await _signInManager.PasswordSignInAsync(context.UserName, context.Password,false, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                var user =await _userRepository.AppUserCrudService.GetAsync("Where IsActive=1 and Email="+context.UserName);
                context.Result = new GrantValidationResult(user.IdentityUserId.ToString(), OidcConstants.AuthenticationMethods.Password);
            }
            //if (_userRepository.ValidateCredentials(context.UserName, context.Password))
            //{
            //    var user = _userRepository.FindByUsername(context.UserName);
            //    context.Result = new GrantValidationResult(user.SubjectId, OidcConstants.AuthenticationMethods.Password);
            //}

            await Task.FromResult(0);
        }
    }
}