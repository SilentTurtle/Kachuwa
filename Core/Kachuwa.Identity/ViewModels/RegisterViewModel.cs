using System.ComponentModel.DataAnnotations;
using Kachuwa.Identity.Models;

namespace Kachuwa.Identity.ViewModels
{
    public class RegisterViewModel:AppUser
    {
       

        [Required]
        [StringLength(100, ErrorMessage = "User.Password.Length", MinimumLength = 8)]
     
        public string Password { get; set; }
    
      
        [Compare("Password", ErrorMessage = "User.Password.NotMatched")]
        public string ConfirmPassword { get; set; }
    }
}