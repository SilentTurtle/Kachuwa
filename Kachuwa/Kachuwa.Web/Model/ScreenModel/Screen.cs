using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kachuwa.Web.Model
{
    public class Screen
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int ModuleId { get; set; }
        public int AddApprovalLevel { get; set; }
        public int EditApprovalLevel { get; set; }
        public int DeleteApprovalLevel { get; set; }
        public string ApprovalUrl { get; set; }
        public string RejectUrl { get; set; }

    }
}
