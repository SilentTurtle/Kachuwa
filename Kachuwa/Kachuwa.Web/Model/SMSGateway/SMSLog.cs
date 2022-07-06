using Kachuwa.Data.Crud.Attribute;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Kachuwa.Web.Model
{
    [Table("SMSLog")]
    public class SMSLog
    {

        [Key]
        public long SMSLogId { get; set; }
      
        public string From { get; set; }
        public string To { get; set; }
        public string Body { get; set; }
        public string GatewayResponse { get; set; }
        
        public DateTime SentDate { get; set; }
        public DateTime DeliveredDate { get; set; }
       
        public bool IsSent { get; set; }
        public bool IsDelivered { get; set; }
        
        [AutoFill(AutoFillProperty.CurrentUser)]
        [IgnoreUpdate]
        public string AddedBy { get; set; }
        
       
        [AutoFill(AutoFillProperty.CurrentDate)]
        [IgnoreUpdate]
        public DateTime AddedOn { get; set; }

        [IgnoreAll]
        public int RowTotal { get; set; }
    }
}
