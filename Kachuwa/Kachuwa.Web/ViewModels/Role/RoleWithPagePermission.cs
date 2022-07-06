using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kachuwa.Identity.Models;
using Kachuwa.Web;

namespace Kachuwa.Web.ViewModels
{
    public class RoleWithPagePermission:IdentityRole
    {
        public List<MasterRolePagePermission> Permissions { get; set; }=new List<MasterRolePagePermission>();

        //for temporary role usage only
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Narration { get; set; }
    }
    public class UpdateRoleWithPagePermission
    {
        public List<MasterRolePagePermission> Permissions { get; set; } = new List<MasterRolePagePermission>();

        public int RoleId { get; set; }
        public long AddedBy { get; set; }
    }
}
