using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kachuwa.Data.Crud.Attribute;

namespace Kachuwa.Identity.Models
{
    [Table("UserLoginHistory")]
    public class UserLoginHistory
    {
        [Key]
        public int UserLoginHistoryId { get; set; }
        public long UserId { get; set; }
        public string IpAddress { get; set; }
        public DateTime LastLogin { get; set; }
        public bool IsFromWeb { get; set; }
        public bool IsFromMobile { get; set; }
        public string UserDevice { get; set; }
        public string Browser { get; set; }
        public string Device { get; set; }

        [AutoFill(AutoFillProperty.CurrentUser)]
        [IgnoreUpdate]
        public string AddedBy { get; set; }
        [AutoFill(AutoFillProperty.CurrentUser)]
        [IgnoreInsert]
        public string UpDatedBy { get; set; }
        [AutoFill(AutoFillProperty.CurrentDate)]
        [IgnoreInsert]
        public DateTime UpdatedOn { get; set; }
        [AutoFill(AutoFillProperty.CurrentDate)]
        [IgnoreUpdate]
        public DateTime AddedOn { get; set; }
        [IgnoreInsert]
        [AutoFill(false)]
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        [IgnoreAll]
        public int RowTotal { get; set; }
    }
}