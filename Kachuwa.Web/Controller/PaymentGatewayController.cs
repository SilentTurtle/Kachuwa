//using System;
//using System.Collections.Generic;
//using System.Collections.Specialized;
//using System.ComponentModel;
//using System.Linq;
//using System.Threading.Tasks;
//using Kachuwa.Identity.Extensions;
//using Kachuwa.Plugin;
//using Kachuwa.Web.Payment;
//using Kachuwa.Web.Service;
//using Microsoft.AspNetCore.Mvc;
//using MySqlX.XDevAPI;

//namespace Kachuwa.Web
//{

//    public class PaymentGatewayController : BaseController
//    {

//        private PaymentGatewayContainer _paymentGatewayContainer;
//        private readonly IPaymentService _iPaymentService;
//        private readonly ICartService _cartService;
//        private readonly IOrderService _orderService;
//        private readonly IEmailManager _emailManager;
//        private readonly IEnumerable<IPlugin> _plugins;
//        public PaymentGatewayController(
//            IPaymentService iPaymentService, ICartService cartService, IOrderService orderService, IEmailManager emailManager)
//        {

//            _iPaymentService = iPaymentService;
//            _cartService = cartService;
//            _orderService = orderService;
//            _emailManager = emailManager;
//            //  _plugins = AutofacDependencyResolver.Current.ApplicationContainer.ResolveKeyed<IEnumerable<IPlugin>>("plugins");
//            // _paymentGatewayContainer = new PaymentGatewayContainer(_plugins);
//        }

//        // GET: /<controller>/
//        [Route("Payment/Process/{gatewayName}")]
//        // GET: /<controller>/
//        public async Task<ActionResult> Process(string gatewayName)
//        {
//            try
//            {
//                //find sys setting from gatewayName
//                //if not found redirect to page not found or invalid
//                //get request payment
//                if (await _iPaymentService.CheckActive(gatewayName))
//                {
//                    var iGateway = _paymentGatewayContainer.Find(gatewayName);
//                    var igatewayMethods = (IPaymentMethod)iGateway;
//                    //get it from session or temporary storage
//                    var paymentReq = Session.Get<PaymentRequest>("PaymentRequest");
//                    if (igatewayMethods.IsPostProcess)
//                    {//need to pass  _iPaymentService in method
//                     //change in pgsamples cs also 
//                        return igatewayMethods.PostProcessPayment(paymentReq, HttpContext);
//                    }
//                    else
//                    {
//                        var status = igatewayMethods.ProcessPayment(paymentReq);
//                        if (!status.IsError)
//                        {
//                            string txnId = status.TransactionId;
//                            //::TODO update order

//                            return RedirectToAction("Success");
//                        }
//                        return RedirectToAction("error");
//                    }

//                }
//                return RedirectToAction("error", new { errors = "Selected payment gateway not found!" });

//            }
//            catch (Exception ex)
//            {
//                return RedirectToAction("error", new { errors = ex.Message.ToString() });
//            }

//        }

//        public async Task SendEmail(int orderId)
//        {
//            OrderHelper orderHelper = new OrderHelper();
//            OrderDomain orderInfo = (OrderDomain)await orderHelper.GetOrderInfo(orderId);
//            var emailTemplate =
//                new EmailTemplateManager().GetRenderedTemplate(new EmailTemplateNames().OrderTemplate(),
//                    orderInfo);
//            _emailManager.SendEmail(new List<string>() { orderInfo.BillingAddress.Email }, emailTemplate);

//        }
//        [Route("Payment/Success/{gatewayName}")]
//        // [Produces("application/x-www-form-urlencoded")]
//        public async Task<ActionResult> Success(string gatewayName)
//        {
//            try
//            {
//                /* this method is invoked when payment is done and redirect to this*/
//                var orderInfo = this.Session.Get<OrderViewModel>("OrderRequest");
//                if (orderInfo == null)
//                {
//                    return RedirectToAction("index", "home");
//                }
//                var iGateway = _paymentGatewayContainer.Find(gatewayName);
//                var igatewayStatus = (IPaymentStatus)iGateway;
//                var paymentResponse = new PaymentResponseStatus();
//                if (HttpContext.Request.HttpMethod == "POST")
//                    paymentResponse = igatewayStatus.Success(HttpContext.Request.Form.ToDictionary<string, string>(), HttpContext.Request.QueryString.ToString());
//                else
//                    paymentResponse = igatewayStatus.Success(null, HttpContext.Request.QueryString.ToString());

//                if (!paymentResponse.IsError)
//                {
//                    //get order info from session
//                    //::TODO update order
//                    //clear cart
//                    //add log
//                    //send email
//                    //show thankyou note with order details
//                    await _cartService.ClearCart(UserHelper.GetCustomerId());

//                    var orderDetails = _orderService.OrderCrud.Get(orderInfo.OrderInfo.OrderId);
//                    var orderCustomerDetail = await _orderService.GetCustomerDetailForOrder(orderInfo.OrderInfo.OrderId);
//                    var orderProductList = await _orderService.GetProductsForOrder(orderInfo.OrderInfo.OrderId);
//                    var orderBillingAddressDetails = await _orderService.GetBillingAddressForOrder(orderInfo.OrderInfo.OrderId);
//                    var orderShiipingAddressDetails = await _orderService.GetOrderShippingAddressForOrder(orderInfo.OrderInfo.OrderId);
//                    var shippingMethod = await _orderService.GetShippingMethodForOrder(orderInfo.OrderInfo.OrderId);
//                    var paymentGateway = await _orderService.GetPaymentGatewayForOrder(orderInfo.OrderInfo.OrderId);
//                    //var orderStatus = await _orderService.GetOrderStatus(orderInfo.OrderInfo.OrderId);

//                    var paymentStatus = await _orderService.GetPaymentStatus(orderInfo.OrderInfo.OrderId);

//                    ////updating item qty after order 
//                    _orderService.UpdateQuantityAfterOrder(orderInfo.CartItems);
//                    ViewData["OBA"] = orderBillingAddressDetails.FirstOrDefault();
//                    ViewData["OSA"] = orderShiipingAddressDetails.FirstOrDefault();
//                    ViewData["cust"] = orderCustomerDetail.FirstOrDefault();
//                    ViewData["Order"] = orderDetails;
//                    ViewData["OrderItems"] = orderProductList;
//                    ViewData["shippingStatus"] = shippingMethod.FirstOrDefault();
//                    ViewData["paymentMethod"] = paymentGateway.FirstOrDefault();
//                    ViewData["paymentStatus"] = paymentStatus.FirstOrDefault();

//                    //updating record of transaction
//                    //::TODO pass gateway responses
//                    //_iPaymentService.UpdateSuccessLog(orderInfo);
//                    await SendEmail(orderInfo.OrderInfo.OrderId);
//                    this.Session.Remove("OrderRequest");
//                    this.Session.Remove("PaymentRequest");

//                }
//                return View();

//            }
//            catch (Exception ex)
//            {
//                return RedirectToAction("error", new { errors = ex.Message.ToString() });
//            }

//        }



//        [Route("Payment/Successtest/{orderid}")]
//        // [Produces("application/x-www-form-urlencoded")]
//        public async Task<ActionResult> SuccessText(int orderId)
//        {
//            try
//            {

//                //get order info from session
//                //::TODO update order
//                //clear cart
//                //add log
//                //send email
//                //show thankyou note with order details
//                //_cartService.ClearCart(UserHelper.GetCustomerId());

//                var orderDetails = _orderService.OrderCrud.Get(orderId);
//                var orderCustomerDetail = await _orderService.GetCustomerDetailForOrder(orderId);
//                var orderProductList = await _orderService.GetProductsForOrder(orderId);
//                var orderBillingAddressDetails = await _orderService.GetBillingAddressForOrder(orderId);
//                var orderShiipingAddressDetails = await _orderService.GetOrderShippingAddressForOrder(orderId);
//                var shippingMethod = await _orderService.GetShippingMethodForOrder(orderId);
//                var paymentGateway = await _orderService.GetPaymentGatewayForOrder(orderId);
//                //var orderStatus = await _orderService.GetOrderStatus(orderId);

//                var paymentStatus = await _orderService.GetPaymentStatus(orderId);

//                ////updating item qty after order 
//                //_orderService.UpdateQuantityAfterOrder(orderProductList);
//                ViewData["OBA"] = orderBillingAddressDetails.FirstOrDefault();
//                ViewData["OSA"] = orderShiipingAddressDetails.FirstOrDefault();
//                ViewData["cust"] = orderCustomerDetail.FirstOrDefault();
//                ViewData["Order"] = orderDetails;
//                ViewData["OrderItems"] = orderProductList;
//                ViewData["shippingStatus"] = shippingMethod.FirstOrDefault();
//                ViewData["paymentMethod"] = paymentGateway.FirstOrDefault();
//                ViewData["paymentStatus"] = paymentStatus.FirstOrDefault();

//                //updating record of transaction
//                //::TODO pass gateway responses
//                //_iPaymentService.UpdateSuccessLog(orderInfo);




//                return View("Success");

//            }
//            catch (Exception ex)
//            {
//                return RedirectToAction("error", new { errors = ex.Message.ToString() });
//            }

//        }

//        [Route("Payment/Verify/{gatewayName}")]
//        public bool Verify(string gatewayName)
//        {
//            try
//            {  //this method will not have any session it will be called from gateway api inorder to verify

//                //update order
//                //clear cart
//                //add log
//                //get order data from session
//                var iGateway = _paymentGatewayContainer.Find(gatewayName);
//                var igatewayStatus = (IPaymentStatus)iGateway;
//                PaymentResponseStatus paymentResponse;
//                if (HttpContext.Request.HttpMethod == "POST")
//                    paymentResponse = igatewayStatus.Verify(HttpContext.Request.Form.ToDictionary<string, string>(), HttpContext.Request.QueryString.ToString());
//                else
//                    paymentResponse = igatewayStatus.Verify(null, HttpContext.Request.QueryString.ToString());

//                //_cartService.ClearCart(0, "");

//                return !paymentResponse.IsError;

//            }
//            catch (Exception ex)
//            {

//                throw ex;
//            }

//        }
//        [Route("Payment/Error/{gatewayName}")]
//        public async Task<ActionResult> Error(string gatewayName, string errors)
//        {
//            try
//            {

//                // var forms = Request.Form;
//                // var iGateway = _paymentGatewayContainer.Find(gatewayName);
//                //var igatewayStatus = (IPaymentStatus)iGateway;
//                //var response = igatewayStatus.Unsuccess(Request.Form, Request.QueryString.ToString());
//                ViewData["Error"] = errors;
//                return View();
//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }

//        }
//        [Route("Payment/Refund/{gatewayName}")]
//        public ActionResult Refund(string gatewayName)
//        {
//            try
//            {
//                //var iGateway = _paymentGatewayContainer.Find(gatewayName);
//                //var igatewayMethod = (IPaymentMethod)iGateway;
//                //var refundReq = Session.Get<PaymentRequest>("RefundRequest");
//                //var response = igatewayMethod.Refund(refundReq);
//                ////show detail and status
//                return View();
//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }

//        }
//    }
//}