using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kachuwa.Data.Crud.Attribute;

namespace Kachuwa.FCM
{
    [Table("UserFCMDevice")]
    public class UserFCMDevice
    {
        [Key]
        public long UserFCMDeviceId { get; set; }
        public long UserId { get; set; }
        public string DeviceId { get; set; }
        public string GroupName { get; set; }
        public string OS { get; set; }
        public string Version { get; set; }
        public bool IsActive { get; set; }
        [IgnoreUpdate]
        [AutoFill(AutoFillProperty.CurrentUserId)]
        public int AddedBy { get; set; }


        [AutoFill(AutoFillProperty.CurrentDate)]
        [IgnoreUpdate]
        public DateTime AddedOn { get; set; }
        [IgnoreInsert]
        public int UpdatedBy { get; set; }


        [AutoFill(AutoFillProperty.CurrentDate)]
        [IgnoreInsert]
        public DateTime UpdatedOn { get; set; }

        public bool IsDeleted { get; set; }

        [AutoFill(AutoFillProperty.CurrentDate)]
        public DateTime DeletedOn { get; set; }
    }
}