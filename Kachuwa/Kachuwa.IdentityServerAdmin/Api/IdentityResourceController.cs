using System;
using System.Linq;
using System.Threading.Tasks;
using Kachuwa.Identity.Security;
using Kachuwa.IdentityServerAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace IdentityServer4.Admin.Controllers.Api
{
    [Route("api/identity-resource")]
    [SecurityHeaders]
    public class IdentityResourceController : ControllerBase
    {
        private readonly IIdentityAdminService _identityAdminService;
      

        public IdentityResourceController(
            IIdentityAdminService identityAdminService,
            ILoggerFactory loggerFactory) : base(loggerFactory)
        {
            _identityAdminService = identityAdminService;
          
        }

        [HttpPut("{id}/disable")]
        public async Task<IActionResult> DisableAsync(int id)
        {
            var resource = await _identityAdminService.IdentityResourcesService.GetAsync(id);
            if (resource == null)
            {
                return BadRequest(new {Code = 400, Msg = "Resource not exists"});
            }

            resource.Enabled = false;
            await _identityAdminService.IdentityResourcesService.UpdateAsync(resource);
         
            return Ok(new {Code = 200, Msg = "Disable success"});
        }

        [HttpPut("{id}/enable")]
        public async Task<IActionResult> EnableAsync(int id)
        {
            var resource = await _identityAdminService.IdentityResourcesService.GetAsync(id);
            if (resource == null)
            {
                return BadRequest(new {Code = 400, Msg = "Resource not exists"});
            }

            resource.Enabled = true;
            await _identityAdminService.IdentityResourcesService.UpdateAsync(resource);
            return Ok(new {Code = 200, Msg = "Enable success"});
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var resource = await _identityAdminService.IdentityResourcesService.GetAsync(id);
            if (resource == null)
            {
                return BadRequest(new {Code = 400, Msg = "Resource not exists"});
            }

            try
            {
             
                var claims =
                    await _identityAdminService.IdentityClaimsService.GetListAsync(
                        "Where IdentityResourceId=@IdentityResourceId", new {IdentityResourceId = id});
                foreach (var claim in claims)
                {
                    await _identityAdminService.IdentityClaimsService.DeleteAsync(claim.Id);
                }
               
                await _identityAdminService.IdentityResourcesService.DeleteAsync(resource.Id);
                
             
            }
            catch (Exception e)
            {
                Logger.LogError($"Delete identity resource failed: {e}");
                try
                {
                   
                }
                catch (Exception te)
                {
                    Logger.LogError($"Rollback delete identity resource failed: {te}");
                }

                return StatusCode(500, new {Code = 500, Msg = "Delete identity resource failed"});
            }

            return Ok(new {Code = 200, Msg = "Delete success"});
        }
    }
}