using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kachuwa.Data.Crud.Attribute;

namespace Kachuwa.Web
{
    [Table("PagePermission")]
    public class PagePermission
    {
        [Key] 
        public long PagePermissionId { get; set; }

        public long PageId { get; set; }
        public long UserId { get; set; }
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


        public bool IsActive { get; set; }

        [AutoFill(false)]
        public bool IsDeleted { get; set; }

        [AutoFill(AutoFillProperty.CurrentDate)]
        [IgnoreUpdate]
        public DateTime AddedOn { get; set; }

        [AutoFill(AutoFillProperty.CurrentUserId)]
        [IgnoreUpdate]
        public long AddedBy { get; set; }


        [AutoFill(AutoFillProperty.CurrentUserId)]
        [IgnoreUpdate]
        public long DeletedBy { get; set; }
        [AutoFill(AutoFillProperty.CurrentDate)]
        [IgnoreInsert]
        public DateTime UpdatedOn { get; set; }
        [AutoFill(AutoFillProperty.CurrentDate)]
        [IgnoreInsert]
        public DateTime DeletedOn { get; set; }

        [AutoFill(AutoFillProperty.CurrentUserId)]
        [IgnoreInsert]
        public long UpdatedBy { get; set; }

        [IgnoreAll]
        public int RowTotal { get; set; }
    }
}