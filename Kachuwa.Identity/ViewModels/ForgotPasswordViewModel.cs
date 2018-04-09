using System.ComponentModel.DataAnnotations;

namespace Kachuwa.Identity.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}