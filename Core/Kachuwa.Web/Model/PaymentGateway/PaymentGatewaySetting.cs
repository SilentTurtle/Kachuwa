using Kachuwa.Data.Crud.Attribute;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kachuwa.Web.Model
{
    [Table("PaymentGatewaySetting")]
    public class PaymentGatewaySetting
    {
        [Key]
        public int PaymentGatewaySettingId { get; set; }
        [IgnoreUpdate]
        public int PaymentGatewayId { get; set; }
        [Required]
        public string SystemName { get; set; }
        [Required]
        public string PaymentGatewayKey { get; set; }
        [Required]
        public string PaymentGatewayValue { get; set; }
    }

    public class PaymentGatewayViewModel
    {
        [Required]
        public IFormFile PluginZipFile { get; set; }
    }
    public class PaymentGatewaySettingViewModel
    {
        public string SystemName { get; set; }
    }
}