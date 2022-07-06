using Microsoft.AspNetCore.Identity;

namespace Kachuwa.IdentityServer.Security;

public class SecurityStampValidatorCallback
{
    public static Task UpdatePrincipal(SecurityStampRefreshingPrincipalContext context)
    {
        var newClaimTypes = context.NewPrincipal.Claims.Select(x => x.Type);
        var currentClaimsToKeep = context.CurrentPrincipal.Claims.Where(x => !newClaimTypes.Contains(x.Type));

        var id = context.NewPrincipal.Identities.First();
        id.AddClaims(currentClaimsToKeep);

        return Task.FromResult(0);
    }
}