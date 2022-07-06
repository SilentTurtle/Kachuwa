using Kachuwa.Data.Crud.Attribute;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Kachuwa.Web.Model
{
    [Table("SMSGatewaySetting")]
    public class SMSGatewaySetting
    {
        [Key]
        public int SMSGatewaySettingId { get; set; }
        [Range(1, Int32.MaxValue)]
        public int SMSGatewayId { get; set; }
        [Required]
        public string GatewayKey { get; set; }
        [Required]
        public string GatewayValue { get; set; }
    }
}
