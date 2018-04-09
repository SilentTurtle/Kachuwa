using System.ComponentModel.DataAnnotations;
using Kachuwa.Identity.Models;

namespace Kachuwa.Identity.ViewModels
{
    public class RegisterViewModel:AppUser
    {
       

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {8} and at max {100} characters long.", MinimumLength = 8)]
        [Display(Name = "Password")]
        public string Password { get; set; }
    
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}