using System;
using System.Collections.Generic;
using Kachuwa.Plugin;
using Microsoft.AspNetCore.Mvc;

namespace Kachuwa.Web.Payment
{
    public interface IPaymentMethod 
    {
        bool SupportRefund { get; }
        bool SupportVoid { get; }
        bool HasSettings { get; }
        bool HasView { get; }
        string ViewPath { get; }
        bool IsPostProcess { get; }
        IList<IPostedValue> PreProcessPayment(IPaymentRequest paymentRequest);
        PaymentResponseStatus ProcessPayment(IPaymentRequest paymentRequest);
        IActionResult PostProcessPayment(IPaymentRequest paymentRequest, object context);
        decimal GetAdditionalFee(int cartId, string sysName);
        bool IsRefundSupported(string sysName);
        IPaymentStatus Refund(IPaymentRequest paymentRequest);
        IPaymentStatus Void(IPaymentRequest paymentRequest);
        IActionResult RenderView(string viewName);

    }
}
