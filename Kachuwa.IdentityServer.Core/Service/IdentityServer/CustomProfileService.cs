using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using IdentityModel;
using Kachuwa.Log;
using Kachuwa.Web;

namespace Kachuwa.IdentityServer.Service
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
            var subjectClaims = context.Subject.Claims;

            //Logger.LogDebug("Get profile called for subject {subject} from client {client} with claim types {claimTypes} via {caller}",
            //context.Subject.GetSubjectId(),
            //context.Client.ClientName ?? context.Client.ClientId,
            //context.RequestedClaimTypes,
            //context.Caller);

            var user = await _appUserService.AppUserCrudService.GetAsync("Where IdentityUserId=@IdentityUserId", new { IdentityUserId = context.Subject.GetSubjectId() });

            var userRoles = await _identityRoleService.GetUserRolesAsync(user.IdentityUserId);

            string img = "";
            if (!string.IsNullOrEmpty(user.ProfilePicture))
                img = user.ProfilePicture;
            var claims = new List<Claim>
            {
                //  new Claim("role", "dataEventRecords.admin"),
                new Claim("IdUid", user.IdentityUserId.ToString()),
                new Claim("email", user.Email),
                new Claim("username", user.UserName??user.Email),
                new Claim("name", user.UserName??user.Email),
                new Claim("firstname", user.FirstName),
                new Claim("surname", user.LastName),
                new Claim("dob", user.DOB??"1988-01-01"),
                new Claim("picture",img??"" ),
              
            };
            claims.AddRange(subjectClaims);
            foreach (var role in userRoles)
            {
                claims.Add(new Claim(JwtClaimTypes.Role, role.Name));
            }
            //var currentIdp = context.Subject.GetIdentityProvider();
            //claims.Add( new Claim("idp", currentIdp ?? "local"));
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