using System.ComponentModel.DataAnnotations;

namespace Kachuwa.IdentityServerAdmin.ViewModel
{
    public class RoleViewModel
    {
        /// <summary>
        /// 角色名称
        /// </summary>
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        
        /// <summary>
        /// 角色描述
        /// </summary>
        [StringLength(500)]
        public string Description { get; set; }
    }
}