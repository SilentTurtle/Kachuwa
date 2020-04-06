using Kachuwa.Web.Model;
using System.Collections.Generic;

namespace Kachuwa.Web.ViewModels
{
    public class MenuViewModel : Menu { 
        public List<MenuPermission> Permissions { get; set; }
    }

    public class MenuOrderViewModel
    {
        public int MenuId { get; set; }
        public int MenuOrder { get; set; }
        public int ParentId { get; set; }
    }

    public class FooterMenuViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Menu> Children { get; set; }
    }
}
