using System.ComponentModel.DataAnnotations;

namespace Kachuwa.IdentityServerAdmin.ViewModel
{
    public class ApiResourceScopeViewModel
    {
        /// <summary>
        /// The unique name of the resource.
        /// </summary>
        [Required]
        [StringLength(200, MinimumLength = 2)]
        public string Name { get; set; }

        /// <summary>
        /// Display name of the resource.
        /// </summary>
        [StringLength(200)]
        public string DisplayName { get; set; }

        /// <summary>
        /// Description of the resource.
        /// </summary>
        [StringLength(1000)]
        public string Description { get; set; }
        
        public bool Required { get; set; }
        
        public bool Emphasize { get; set; }
        
        public bool ShowInDiscoveryDocument { get; set; } = true;
        
        public string UserClaims { get; set; }
    }
}