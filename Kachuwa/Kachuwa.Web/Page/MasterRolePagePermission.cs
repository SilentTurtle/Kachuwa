using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kachuwa.Data.Crud.Attribute;

namespace Kachuwa.Web
{
    [Table("MasterRolePagePermission")]
    public class MasterRolePagePermission
    {
        [Key]
        public long MasterRolePagePermissionId { get; set; }
        public int PageId { get; set; }
        public int RoleId { get; set; }
        public bool AllowAccessForAll { get; set; }
        public bool ListView { get; set; }
        public bool AddNew { get; set; }
        public bool Edit { get; set; }
        public bool Delete { get; set; }
        public bool ChangeStatus { get; set; }
        public bool Download { get; set; }
        public bool Import { get; set; }
        public bool Export { get; set; }
        public bool Print { get; set; }
        public bool ViewReport { get; set; }
        public bool RunReport { get; set; }
        public int ApproveLevel { get; set; }


        [AutoFill(AutoFillProperty.CurrentDate)]
        [IgnoreUpdate]
        public DateTime AddedOn { get; set; }
        [AutoFill(AutoFillProperty.CurrentUser)]
        [IgnoreUpdate]
        public string AddedBy { get; set; }
        [IgnoreInsert]
        public DateTime DeletedOn { get; set; }
        [AutoFill(AutoFillProperty.CurrentUserId)]
        [IgnoreInsert]
        public long DeletedBy { get; set; }

        [AutoFill(AutoFillProperty.CurrentUserId)]
        [IgnoreInsert]
        public long UpdatedBy { get; set; }

        [IgnoreInsert]
        public DateTime UpdatedOn { get; set; }

        [IgnoreAll]
        public string Url { get; set; } = "/";



    }
}