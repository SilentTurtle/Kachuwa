using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kachuwa.Data.Crud.Attribute;
using MySqlX.XDevAPI.Relational;

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
        public string ConnectionId { get; set; }
        public  string HubName { get; set; }
        
    }
}
