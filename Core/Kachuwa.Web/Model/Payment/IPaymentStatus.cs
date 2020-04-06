using System;
using System.Collections.Generic;
using Kachuwa.Plugin;

namespace Kachuwa.Web.Payment
{
    public interface IPaymentStatus : IPlugin
    {
        PaymentResponseStatus Success(Dictionary<string, string> formCollection, string queryStrings);
        PaymentResponseStatus Verify(Dictionary<string, string> formCollection, string queryStrings);
        PaymentResponseStatus Unsuccess(Dictionary<string, string> formCollection, string queryStrings);

    }


}
