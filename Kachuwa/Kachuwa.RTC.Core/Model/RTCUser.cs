using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kachuwa.Data.Crud.Attribute;

namespace Kachuwa.RTC
{
    [Table("RTCUser")]
    public class RTCUser
    {
        [Key]
        public int RTCUserId { get; set; }
        [IgnoreAll]
        public List<string> ConnectionIds { get; set; }=new List<string>();
        public long IdentityUserId { get; set; }
        public string UserRoles { get; set; }
        public string UserDevice { get; set; }
        public string SessionId { get; set; }
        public bool IsFromWeb { get; set; }
        public bool IsFromMobile { get; set; }
        public string GroupName { get; set; }
        public List<string> GroupNames { get; set; } = new List<string>();
        public string ConnectionId { get; set; }
        public List<string> HubNames { get; set; } = new List<string>();
        [IgnoreAll]
        public bool InCall { get; set; }
        [IgnoreAll]
        public string UserName { get; set; }
        [IgnoreAll]
        public string FullName { get; set; }
        [IgnoreAll]
        public string Picture { get; set; }

    }

    public class RtcUserStatus
    {
        public int TotalUser { get; set; }
        public int TotalMobileUser { get; set; }
        public int TotalWebUser { get; set; }
        public int TotalGuestUser { get; set; }
    }
}
