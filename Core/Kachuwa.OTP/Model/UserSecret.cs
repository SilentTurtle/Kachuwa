using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kachuwa.Data.Crud.Attribute;

namespace Kachuwa.OTP.Model
{
    [Table("UserSecretKey")]
    public class UserSecretKey
    {
        [Key]
        public long UserSecretKeyId { get; set; }
        public long UserId { get; set; }
        public string SecretKey { get; set; }

        [AutoFill(AutoFillProperty.CurrentUser)]
        [IgnoreUpdate]
        public string AddedBy { get; set; }

        [AutoFill(AutoFillProperty.CurrentDate)]
        [IgnoreUpdate]
        public DateTime AddedOn { get; set; }


        [AutoFill(AutoFillProperty.CurrentUser)]
        [IgnoreInsert]
        public string UpDatedBy { get; set; }

        [AutoFill(AutoFillProperty.CurrentDate)]
        [IgnoreInsert]
        public DateTime UpdatedOn { get; set; }

        [IgnoreInsert]
        public bool IsDeleted { get; set; }

        [IgnoreAll]
        public int RowTotal { get; set; }

    }
    [Table("OTPSetting")]
    public class OTPSetting
    {
        [Key]
        public int OTPSettingId { get; set; }
        public int ExpiryTime { get; set; }//in seconds
        public bool SendFromSMS { get; set; }
        public string SendFromEmail { get; set; }
        [AutoFill(AutoFillProperty.CurrentUser)]
        [IgnoreUpdate]
        public string AddedBy { get; set; }

        [AutoFill(AutoFillProperty.CurrentDate)]
        [IgnoreUpdate]
        public DateTime AddedOn { get; set; }


        [AutoFill(AutoFillProperty.CurrentUser)]
        [IgnoreInsert]
        public string UpDatedBy { get; set; }

        [AutoFill(AutoFillProperty.CurrentDate)]
        [IgnoreInsert]
        public DateTime UpdatedOn { get; set; }

        [IgnoreInsert]
        public bool IsDeleted { get; set; }

        [IgnoreAll]
        public int RowTotal { get; set; }
    }
    [Table("UserOTP")]
    public class UserOTP
    {
        [Key]
        public long UserOTPId { get; set; }
        public long UserId { get; set; }
        public string OTPCode { get; set; }
        public bool IsExpired { get; set; }
    }
}
