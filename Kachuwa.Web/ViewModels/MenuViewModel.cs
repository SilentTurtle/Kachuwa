using Kachuwa.Web.Model;
using System.Collections.Generic;

namespace Kachuwa.Web.ViewModels
{
    public class MenuViewModel : Menu { 
        public List<MenuPermission> Permissions { get; set; }
    }
}
