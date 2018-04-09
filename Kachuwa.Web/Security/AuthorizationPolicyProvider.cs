using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace Kachuwa.Web.Security
{
    public class AuthorizationPolicyProvider : DefaultAuthorizationPolicyProvider
    {
        private readonly IConfiguration _configuration;

        public AuthorizationPolicyProvider(IOptions<AuthorizationOptions> options, IConfiguration configuration) : base(options)
        {
            _configuration = configuration;
        }

        public override async Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {

            // Check static policies first
            var policy = await base.GetPolicyAsync(policyName);

            if (policy == null)
            {
                //TODO::Dynamic policy retriever

                //policy = new AuthorizationPolicyBuilder()
                //.AddRequirements(new HasScopeRequirement(policyName, $"https://{_configuration["Auth0:Domain"]}/"))
                //.RequireAssertion(context =>
                // context.User.HasClaim(z => z.Type == ClaimTypes.Role && z.Value.Contains("Admin")))
                //.RequireRole("Admin")
                //.RequireScope()
                // .Build();
            }

            return policy;
        }
    }
}
