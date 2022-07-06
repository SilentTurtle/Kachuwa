using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kachuwa.Web.Model
{
    public class ActivityLogInputModel
    {
        public int UserId { get; set; }
        public int ScreenId { get; set; }
        public long RefrenceId { get; set; }
        public string Data { get; set; }
        public long ActivityLogId { get; set; }
    }
    public class ActivityLogResponseModel
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public long ActivityLogId { get; set; }
        public bool AllowActualChange { get; set; }
        public string Data { get; set; }
        public long RefrenceId { get; set; }
        public ActionType ActionType { get; set; }

    }
    public enum ActionType
    {
        New = 'N',
        Edit = 'E',
        Delete = 'D',
        Approve = 'A',
    }
}
