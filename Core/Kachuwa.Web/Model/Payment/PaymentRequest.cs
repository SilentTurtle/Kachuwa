using System.Collections.Generic;

namespace Kachuwa.Web.Payment
{
   

    public class PaymentRequest : IPaymentRequest
    {
        public string IP;

        public PaymentRequest()
        {
            CustomValues = new Dictionary<string, object>();
            this.IsRefundRequest = false;
            this.IsVoidRequest = false;
        }
        public PaymentRequest(bool isRefundRequest, bool isVoidRequest)
        {
            CustomValues = new Dictionary<string, object>();
            this.IsRefundRequest = isRefundRequest;
            this.IsVoidRequest = isVoidRequest;
        }

        public string UserName { get; set; }
       // public List<CartItem> CartItems { get; set; }
        public int CartId { get; set; }
        public long OrderId { get; set; }
        public long UserId { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal Discount { get; set; }
        public decimal TaxTotal { get; set; }
        public decimal ShippingRate { get; set; }
        public string PaymentMethodSysName { get; set; }


        public string CreditCardType { get; set; }
        public string CreditCardName { get; set; }
        public string CreditCardNumber { get; set; }
        public int CreditCardExpireYear { get; set; }
        public int CreditCardExpireMonth { get; set; }
        public string CreditCardCvv2 { get; set; }

        public string PurchaseOrderNumber { get; set; }
        //Recurring fields

        public Dictionary<string, object> CustomValues { get; set; }


        public bool IsRefundRequest { get; set; }
        public bool IsVoidRequest { get; set; }
        public RefundRequest RefundRequest { get; set; }
        public VoidRequest VoidRequest { get; set; }
       // public OrderBillingAddress BillingAddress { get; set; }
       // public OrderShippingAddress ShippingAddress { get; set; }
        public string InvoiceNumber { get; set; }
        public string Currency { get; set; }
        public string GiftCardNumber { get; set; }
        public string GiftCardPin { get; set; }
        public int PaymentGatewayId { get; set; }
    }
}