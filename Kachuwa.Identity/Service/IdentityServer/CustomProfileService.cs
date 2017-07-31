using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Kachuwa.Log;

namespace Kachuwa.Identity.Service
{
    public class CustomProfileService : IProfileService
    {
        private readonly IAppUserService _appUserService;
        protected readonly ILogger Logger;

        public CustomProfileService(IAppUserService appUserService, ILogger logger)
        {
         
            _appUserService = appUserService;
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

            var user = await _appUserService.AppUserCrudService.GetAsync("Where IdentityUserId="+context.Subject.GetSubjectId(),new{});

            var claims = new List<Claim>
            {
                new Claim("role", "dataEventRecords.admin"),
                new Claim("role", "dataEventRecords.user"),
                new Claim("username", user.Email),
                new Claim("email", user.Email)
            };

            context.IssuedClaims = claims;
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var sub = context.Subject.GetSubjectId();
            var user=await _appUserService.AppUserCrudService.GetAsync("Where IsActive=1 and IdentityUserId=" + context.Subject.GetSubjectId(), new { });
            context.IsActive = user != null;
        }
    }
}