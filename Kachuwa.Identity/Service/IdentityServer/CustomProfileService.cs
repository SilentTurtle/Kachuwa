using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Kachuwa.Identity.Extensions;
using Kachuwa.Log;
using Kachuwa.Web;

namespace Kachuwa.Identity.Service
{
    public class CustomProfileService : IProfileService
    {
        private readonly IAppUserService _appUserService;
        private readonly IIdentityRoleService _identityRoleService;
        protected readonly ILogger Logger;

        public CustomProfileService(IAppUserService appUserService, IIdentityRoleService identityRoleService,
            ILogger logger)
        {

            _appUserService = appUserService;
            _identityRoleService = identityRoleService;
            Logger = logger;
        }


        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var sub = context.Subject.GetSubjectId();

            //Logger.LogDebug("Get profile called for subject {subject} from client {client} with claim types {claimTypes} via {caller}",
            //context.Subject.GetSubjectId(),
            //context.Client.ClientName ?? context.Client.ClientId,
            //context.RequestedClaimTypes,
            //context.Caller);

            var user = await _appUserService.AppUserCrudService.GetAsync("Where IdentityUserId=@IdentityUserId", new { IdentityUserId = context.Subject.GetSubjectId() });

            var userRoles = await _identityRoleService.GetUserRolesAsync(Int64.Parse(context.Subject.GetSubjectId()));
           
            var claims = new List<Claim>
            {
              //  new Claim("role", "dataEventRecords.admin"),
                new Claim("IdUid", user.IdentityUserId.ToString()),
                new Claim("username", user.Email),
                new Claim("email", user.Email),
                new Claim("firstname", user.FirstName),
                new Claim("lastname", user.LastName),
                new Claim("dob", user.DOB),
                new Claim("appuserid", user.AppUserId.ToString()),
                //TODO:: get platform dynamically
                new Claim("plat","apps")   //ContextResolver.Context.User.Identity.GetUserPlatform()
            };
            foreach (var role in userRoles)
            {
                claims.Add(new Claim("role", role.Name));
            }

            context.IssuedClaims = claims;
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var sub = context.Subject.GetSubjectId();
            var user = await _appUserService.AppUserCrudService.GetAsync("Where IsActive=1 and IdentityUserId=" + context.Subject.GetSubjectId(), new { });
            context.IsActive = user != null;
        }
    }
}