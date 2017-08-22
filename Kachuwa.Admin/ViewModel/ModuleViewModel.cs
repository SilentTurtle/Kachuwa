using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Kachuwa.Web.Model;
using Kachuwa.Web.Module;

namespace Kachuwa.Admin.ViewModel
{
    public class ModuleViewModel
    {
        
        public string ModuleName { get; set; }
        public bool HasSetting { get; set; }
        public List<ModuleComponentDescription> ModuleComponents { get; set; }=new List<ModuleComponentDescription>();
    }

    public class PageViewModel : SEO
    {
        public long PageId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Url { get; set; }
        public string Content { get; set; }
        public bool UseMasterLayout { get; set; }
        public bool IsPublished { get; set; }
    }
}