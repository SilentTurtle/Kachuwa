using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kachuwa.Web.Payment
{
   public class PaymentResponseStatus
    {
        public IList<string> Erors { get; set; }=new List<string>();

        public bool IsError { get; set; }

        public PaymentStatus PaymentStatus { get; set; }

        public string TransactionId { get; set; }

        public decimal PaidAmount { get; set; }

        public string PayerFullName { get; set; }

        public string PayerEmail { get; set; }
        public string PayerMobile { get; set; }

        public string GatewayResponse { get; set; }
        public string UserName { get; set; }
        public long UserId { get; set; }
        public long OrderId { get; set; }
        public string CustomValues { get; set; }

    }
}
