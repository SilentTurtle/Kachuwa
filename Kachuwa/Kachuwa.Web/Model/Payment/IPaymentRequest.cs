using System.Collections.Generic;

namespace Kachuwa.Web.Payment
{
    public interface IPaymentRequest
    {
        //OrderBillingAddress BillingAddress { get; set; }
        int CartId { get; set; }
        string CreditCardCvv2 { get; set; }
        int CreditCardExpireMonth { get; set; }
        int CreditCardExpireYear { get; set; }
        string CreditCardName { get; set; }
        string CreditCardNumber { get; set; }
        string CreditCardType { get; set; }
        string Currency { get; set; }
        Dictionary<string, object> CustomValues { get; set; }
        decimal Discount { get; set; }
        string InvoiceNumber { get; set; }
        bool IsRefundRequest { get; set; }
        bool IsVoidRequest { get; set; }
        long OrderId { get; set; }
        decimal GrandTotal { get; set; }
        string PaymentMethodSysName { get; set; }
        string PurchaseOrderNumber { get; set; }
        RefundRequest RefundRequest { get; set; }
       // OrderShippingAddress ShippingAddress { get; set; }
        decimal ShippingRate { get; set; }
        decimal TaxTotal { get; set; }
        long UserId { get; set; }
        string UserName { get; set; }
        VoidRequest VoidRequest { get; set; }
        string Other { get; set; }
        string TransactionNumber { get; set; }
    }
}