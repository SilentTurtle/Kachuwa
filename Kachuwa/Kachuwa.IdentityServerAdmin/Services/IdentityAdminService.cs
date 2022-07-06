using Kachuwa.Data;
using Kachuwa.IdentityServerAdmin.Model;

namespace Kachuwa.IdentityServerAdmin.Services
{
    public class IdentityAdminService : IIdentityAdminService
    {
        public CrudService<ApiClaims> ApiClaimsService { get; set; } = new CrudService<ApiClaims>();
        public CrudService<ApiResources> ApiResourcesService { get; set; }=new CrudService<ApiResources>();
        public CrudService<ApiScopeClaims> ApiScopeClaimsService { get; set; }=new CrudService<ApiScopeClaims>();
        public CrudService<ApiScopes> ApiScopesService { get; set; }=new CrudService<ApiScopes>();
        public CrudService<ApiSecrets> ApiSecretsService { get; set; }=new CrudService<ApiSecrets>();
        public CrudService<ClientClaims> ClientClaimsService { get; set; }=new CrudService<ClientClaims>();

        public CrudService<ClientCorsOrigins> ClientCorsOriginsService { get; set; }=new CrudService<ClientCorsOrigins>();
        public CrudService<ClientGrantTypes> ClientGrantTypesService { get; set; }=new CrudService<ClientGrantTypes>();

        public CrudService<ClientIdPRestrictions> ClientIdPRestrictionsService { get; set; } =
            new CrudService<ClientIdPRestrictions>();
        public CrudService<ClientPostLogoutRedirectUris> ClientPostLogoutRedirectUrisService { get; set; }=new CrudService<ClientPostLogoutRedirectUris>();
        public CrudService<ClientProperties> ClientPropertiesService { get; set; }=new CrudService<ClientProperties>();
        public CrudService<ClientRedirectUris> ClientRedirectUrisService { get; set; }=new CrudService<ClientRedirectUris>();
        public CrudService<Clients> ClientsService { get; set; } = new CrudService<Clients>();
        public CrudService<ClientScopes> ClientScopesService { get; set; } = new CrudService<ClientScopes>();
        public CrudService<ClientSecrets> ClientSecretsService { get; set; } = new CrudService<ClientSecrets>();
        public CrudService<IdentityClaims> IdentityClaimsService { get; set; }=new CrudService<IdentityClaims>();
        public CrudService<IdentityResources> IdentityResourcesService { get; set; }=new CrudService<IdentityResources>();
    }
}