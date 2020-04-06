using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kachuwa.Data.Crud.Attribute;

namespace Kachuwa.Web
{
    [Table("EmailLog")]
    public class EmailLog
    {
        [Key]
        public long EmailLogId { get; set; }
        [Required]
        public string From { get; set; }
        [Required]
        public string To { get; set; }
        [Required]
        public string Subject { get; set; }
        [Required]
        public string Body { get; set; }

        public string CC { get; set; }

        public string BCC { get; set; }

        public bool IsSent { get; set; }

        public bool IsDelivered { get; set; }

        public DateTime SentDate { get; set; }

        public DateTime DeliveredDate { get; set; }

        public string GatewayResponse { get; set; }

        [AutoFill(AutoFillProperty.CurrentDate)]
        public DateTime AddedOn { get; set; }
        [AutoFill(AutoFillProperty.CurrentUser)]
        public string AddedBy { get; set; }
        [IgnoreAll]
        public int RowTotal { get; set; }
    }
}