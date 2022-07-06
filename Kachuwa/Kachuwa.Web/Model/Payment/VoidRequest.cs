using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kachuwa.Web.Payment
{
    public class VoidRequest
    {

    }
    [Table("PaymentTransactionLog")]
    public class PaymentTransactionLog
    {

        [Key]
        public long PaymentTransactionLogId { get; set; }

        public int CartId { get; set; }

        public int PaymentGatewayId { get; set; }
        public string PaymentGatewayName { get; set; }

        public int TheaterId { get; set; }

        public decimal Amount { get; set; }

        public  decimal PaidAmount { get; set; }

        public string PaidByUser { get; set; }

        public string GatewayResponse { get; set; }

        public bool IsError { get; set; }

        public string TransactionId { get; set; }

        public DateTime AddedOn { get; set; }

        public string AddedBy { get; set; }
    }
}