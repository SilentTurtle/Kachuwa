using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kachuwa.Identity.Models;
using Kachuwa.Web;

namespace Kachuwa.Web.ViewModels
{
    public class PageWithPagePermission:Page
    {
        public List<PagePermission> Permissions { get; set; }=new List<PagePermission>();
        public long UserId { get; set; }
    }
}
