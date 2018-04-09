using System.ComponentModel.DataAnnotations;

namespace Kachuwa.Identity.ViewModels
{
    public class ExternalLoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}