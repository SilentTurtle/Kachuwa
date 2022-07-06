using System.ComponentModel.DataAnnotations.Schema;

namespace Kachuwa.IdentityServerAdmin.Model
{
    [Table("Clients")]
    public class Clients
    {
        public int Id { get; set; }
        public int AbsoluteRefreshTokenLifetime { get; set; }
        public int AccessTokenLifetime { get; set; }
        public int AccessTokenType { get; set; }
        public int AllowAccessTokensViaBrowser { get; set; }
        public int AllowOfflineAccess { get; set; }
        public int AllowPlainTextPkce { get; set; }
        public int AllowRememberConsent { get; set; }
        public int AlwaysIncludeUserClaimsInIdToken { get; set; }
        public int AlwaysSendClientClaims { get; set; }
        public int AuthorizationCodeLifetime { get; set; }
        public int BackChannelLogoutSessionRequired { get; set; }
        public string BackChannelLogoutUri { get; set; }
        public string ClientClaimsPrefix { get; set; }
        public string ClientId { get; set; }
        public string ClientName { get; set; }
        public string ClientUri { get; set; }
        public int ConsentLifetime { get; set; }
        public string Description { get; set; }
        public int EnableLocalLogin { get; set; }
        public int Enabled { get; set; }
        public int FrontChannelLogoutSessionRequired { get; set; }
        public string FrontChannelLogoutUri { get; set; }
        public int IdentityTokenLifetime { get; set; }
        public int IncludeJwtId { get; set; }
        public string LogoUri { get; set; }
        public string PairWiseSubjectSalt { get; set; }
        public string ProtocolType { get; set; }
        public int RefreshTokenExpiration { get; set; }
        public int RefreshTokenUsage { get; set; }
        public int RequireClientSecret { get; set; }
        public int RequireConsent { get; set; }
        public int RequirePkce { get; set; }
        public int SlidingRefreshTokenLifetime { get; set; }
        public int UpdateAccessTokenClaimsOnRefresh { get; set; }
    }
}