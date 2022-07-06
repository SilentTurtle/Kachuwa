using System.ComponentModel.DataAnnotations.Schema;

namespace Kachuwa.IdentityServerAdmin.Model
{
    [Table("ApiScopes")]
    public class ApiScopes
    {
        public int Id { get; set; }
        public int ApiResourceId { get; set; }
        public string Description { get; set; }
        public string DisplayName { get; set; }
        public bool Emphasize { get; set; }
        public string Name { get; set; }
        public bool Required { get; set; }
        public bool ShowInDiscoveryDocument { get; set; }
    }
}