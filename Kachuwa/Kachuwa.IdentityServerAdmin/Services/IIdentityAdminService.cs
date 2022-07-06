using Kachuwa.Data;
using Kachuwa.IdentityServerAdmin.Model;

namespace Kachuwa.IdentityServerAdmin.Services
{
    public interface IIdentityAdminService
    {
        CrudService<ApiClaims> ApiClaimsService { get; set; }
        CrudService<ApiResources> ApiResourcesService { get; set; }
        CrudService<ApiScopeClaims> ApiScopeClaimsService { get; set; }
        CrudService<ApiScopes> ApiScopesService { get; set; }
        CrudService<ApiSecrets> ApiSecretsService { get; set; }
        CrudService<ClientClaims> ClientClaimsService { get; set; }
        CrudService<ClientCorsOrigins> ClientCorsOriginsService { get; set; }
        CrudService<ClientGrantTypes> ClientGrantTypesService { get; set; }
        CrudService<ClientIdPRestrictions> ClientIdPRestrictionsService { get; set; }
        CrudService<ClientPostLogoutRedirectUris> ClientPostLogoutRedirectUrisService { get; set; }
        CrudService<ClientProperties> ClientPropertiesService { get; set; }
        CrudService<ClientRedirectUris> ClientRedirectUrisService { get; set; }
        CrudService<Clients> ClientsService { get; set; }
        CrudService<ClientScopes> ClientScopesService { get; set; }
        CrudService<ClientSecrets> ClientSecretsService { get; set; }
        CrudService<IdentityClaims> IdentityClaimsService { get; set; }
        CrudService<IdentityResources> IdentityResourcesService { get; set; }
    }
}