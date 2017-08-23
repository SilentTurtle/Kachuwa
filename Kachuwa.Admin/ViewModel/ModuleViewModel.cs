using System.Collections.Generic;
using Kachuwa.Web.Module;

namespace Kachuwa.Admin.ViewModel
{
    public class ModuleViewModel
    {
        
        public string ModuleName { get; set; }
        public bool HasSetting { get; set; }
        public List<ModuleComponentDescription> ModuleComponents { get; set; }=new List<ModuleComponentDescription>();
    }
}