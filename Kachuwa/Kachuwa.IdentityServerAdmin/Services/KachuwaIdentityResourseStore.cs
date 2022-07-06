using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Kachuwa.Log;

namespace Kachuwa.IdentityServerAdmin.Services
{
    public class KachuwaIdentityResourseStore : IResourceStore
    {
        private readonly IIdentityAdminService _identityAdminService;
        private readonly ILogger _logger;

        public KachuwaIdentityResourseStore(IIdentityAdminService identityAdminService,ILogger logger)
        {
            _identityAdminService = identityAdminService;
            _logger = logger;
        }
        public Task<IEnumerable<IdentityResource>> FindIdentityResourcesByScopeAsync(IEnumerable<string> scopeNames)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ApiResource>> FindApiResourcesByScopeAsync(IEnumerable<string> scopeNames)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResource> FindApiResourceAsync(string name)
        {
            throw new NotImplementedException();
        }

        public Task<Resources> GetAllResourcesAsync()
        {
            throw new NotImplementedException();
        }
    }
}
