using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kachuwa.Web.Module;

namespace Kachuwa.Web.ViewModels.Module
{
    public class ModuleWithPages:ModuleInfo
    {

        public List<PageWithPagePermission> Pages { get; set; } = new List<PageWithPagePermission>();
    }
}
