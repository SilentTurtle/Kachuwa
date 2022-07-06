using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kachuwa.IdentityServerAdmin.Model
{
    [Table("IdentityResources")]
    public class IdentityResources
    {
        [Key]
        public int Id { get; set; }
        public string Description { get; set; }
        public string DisplayName { get; set; }
        public int Emphasize { get; set; }
        public bool Enabled { get; set; }
        public string Name { get; set; }
        public bool Required { get; set; }
        public bool ShowInDiscoveryDocument { get; set; }
    }
}