using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Kachuwa.Plugin
{
    public class PluginViewModel
    {
        [Required]
        public IFormFile PluginZipFile { get; set; }

        public string SystemName { get; set; }
    }
}