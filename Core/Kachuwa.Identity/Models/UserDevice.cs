using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kachuwa.Data.Crud.Attribute;

namespace Kachuwa.Identity.Models
{
    [Table("UserDevice")]
    public class UserDevice
    {
        [Key]
        public long UserDeviceId { get; set; }
        public long UserId { get; set; }
        public string DeviceId { get; set; }
        public bool IsWeb { get; set; }
        public bool IsMobile { get; set; }
        public string Browser { get; set; }
        public string BrowserVersion { get; set; }
        public string OS { get; set; }
        public string Version { get; set; }
        public bool IsVerified { get; set; }
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

    public class UserDeviceStatus
    {
        public bool MobileDevice { get; set; }
        public int VerifiedDeviceCount { get; set; }
        public bool IsThisUnverifiedLogin { get; set; } 
        public int BrowserCount { get; set; }
        public int MobileCount { get; set; }
    }

    public class DeviceVerificationStatus
    {
        public bool IsVerified { get; set; }
        public string Message { get; set; }
    }
    public class DeviceRemovalViewModel
    {
        [Required]
        public long UserDeviceId { get; set; }
    }
}