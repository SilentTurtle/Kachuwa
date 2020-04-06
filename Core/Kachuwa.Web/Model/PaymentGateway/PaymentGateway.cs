using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kachuwa.Data.Crud.Attribute;
using Kachuwa.Plugin;

namespace Kachuwa.Web.Model
{
    [Table("PaymentGateway")]
    public class PaymentGateway
    {
        [Key]
        public int PaymentGatewayId { get; set; }
        [Required(ErrorMessage ="PaymentGateway.Name.Required")]
        public string PaymentGatewayName { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        [Required(ErrorMessage = "PaymentGateway.SystemName.Required")]
        public string SystemName { get; set; }

        public string Type { get; set; }//web or mobile /wallet

        public bool IsActive { get; set; }
        [AutoFill(AutoFillProperty.CurrentDate)]
        [IgnoreUpdate]
        public DateTime AddedOn { get; set; }
        [AutoFill(AutoFillProperty.CurrentUser)]
        [IgnoreUpdate]
        public string AddedBy { get; set; }
       
        [IgnoreAll]
        public int RowTotal { get; set; }
    }

    public class PaymentGatewayWithConfig: PaymentGateway
    {
        [IgnoreAll]
        public IPlugin Plugin { get; set; }
    }

    public class PgStatus
    {
        public bool IsInstalled { get; set; }
        public bool HasError { get; set; }
        public string ErrorMessage { get; set; }
    }
    public static class PaymentGatewayTypes
    {
        public const string WEB = "WEB";
        public const string MOBILE = "MOBILE";
    }
}