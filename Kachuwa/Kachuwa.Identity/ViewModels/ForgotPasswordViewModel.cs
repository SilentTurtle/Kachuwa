using System.ComponentModel.DataAnnotations;

namespace Kachuwa.Identity.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        public string EmailOrUserName { get; set; }
    }
}