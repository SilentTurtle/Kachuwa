using Kachuwa.Identity.Models;
using Kachuwa.Identity.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Kachuwa.API.ViewModels
{
    public class UserRegisterViewModel : AppUser
    {
        public int BranchId { get; set; }
        public int[] Roles { get; set; }
        public string Password { get; set; }
    }
}
