using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Kachuwa.Web.Theme
{
    public class ThemeViewModel
    {
        [Required]
        public IFormFile ThemeZip { get; set; }
    }

    public class ThemeStatus
    {
        public bool IsInstalled { get; set; }
        public string Error { get; set; }
        public bool HasError { get; set; }

    }
}