using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kachuwa.Localization;
using Kachuwa.Plugin;
using Kachuwa.Web;
using Kachuwa.Web.Model;
using Kachuwa.Web.Notification;
using Kachuwa.Web.Payment;
using Kachuwa.Web.Security;
using Kachuwa.Web.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace Kachuwa.Admin.Controllers
{
   
    [Area("Admin")]
    //[Authorize(PolicyConstants.PagePermission)]
    public class PaymentGatewayController : BaseController
    {
        private readonly IPaymentService _paymentService;
        private readonly IApplicationLifetime _applicationLifetime;
        private readonly IEnumerable<IPlugin> _plugins;
        private IEnumerable<PaymentGateway> _installedGateways;
        private readonly IPluginProvider _pluginProvider;
        private readonly INotificationService _notificationService;
        private readonly ILocaleResourceProvider _localeResourceProvider;

        public PaymentGatewayController(IPaymentService paymentService,IApplicationLifetime applicationLifetime,
            IPluginProvider pluginProvider,
            INotificationService  notificationService,ILocaleResourceProvider localeResourceProvider)
        {
            _paymentService = paymentService;
            _applicationLifetime = applicationLifetime;
            _pluginProvider = pluginProvider;
            _notificationService = notificationService;
            _localeResourceProvider = localeResourceProvider;
            _plugins = _pluginProvider.Plugins;
            _localeResourceProvider.LookUpGroupAt("PaymentGateway");
        }

        [Route("admin/paymentgateway")]
        public async Task<ActionResult> Index()
        {
            if (_plugins != null)
            {
                var paymentGateways =
                    _plugins.Where(e => e.Configuration.PluginType == PluginType.PaymentGateway).ToList();

                _installedGateways = await _paymentService.GetAllPaymentGateways();

                paymentGateways = paymentGateways.Select(e =>
                {
                    if (_installedGateways.Any(x => x.SystemName == e.SystemName && x.IsActive==true))
                    {
                        e.Configuration.IsInstalled = true;
                        
                        return e;
                    }
                    else
                    {
                        return e;
                    }
                }).ToList();

                return View(paymentGateways);
            }
            return View(new List<IPlugin>());
        }

        [Route("admin/paymentgateway/setting/{sysName}")]
        [Route("admin/paymentgateway/setting")]
        public async Task<ActionResult> Setting([FromQuery][FromRoute]string sysName)
        {
            if (string.IsNullOrEmpty(sysName))
                return Redirect("/page-not-found");

            return View(new PaymentGatewaySettingViewModel { SystemName = sysName });
        }

        public async Task<ActionResult> New()
        {
            return View();
        }
      
        [HttpPost]
        public async Task<ActionResult> New(PaymentGatewayViewModel model)
        {
            if (model.PluginZipFile != null)
            {
              var status=await  _paymentService.UnzipAndInstall(model.PluginZipFile);
                if (status.IsInstalled)
                {
                    _notificationService.Notify(_localeResourceProvider.Get("Success"),
                        _localeResourceProvider.Get("PaymentGateway.InstallMessage"), NotificationType.Success);
                    _notificationService.Notify(_localeResourceProvider.Get("Info"),
                        _localeResourceProvider.Get("PaymentGateway.ConfiguringMessage"), NotificationType.Info);

                    await Task.Delay(1000);
                    _applicationLifetime.StopApplication();
                }
                else
                {
                    _notificationService.Notify(_localeResourceProvider.Get("Error"),
                        status.ErrorMessage, NotificationType.Error);

                }
            }
            else
            {
                _notificationService.Notify(_localeResourceProvider.Get("Alert"), _localeResourceProvider.Get("PaymentGateway.UploadZipFileFirst"),
                    NotificationType.Warning);
                ModelState.AddModelError("PluginZipFile", "Please upload file first.");
            }

            return View();
        }
        public async Task<IActionResult> LoadSetting(string sysName)
        {
            var paymentGateways = _plugins.Where(e => e.Configuration.PluginType == PluginType.PaymentGateway).ToList();
            //_installedGateways = await _paymentService.GetAllPaymentGateways();

            var plugin = paymentGateways.Where(x => x.SystemName == sysName).SingleOrDefault();
            var paymentSys = (IPaymentMethod)plugin;
            return paymentSys.RenderView("");
        }

        [HttpPost]
        public async Task<JsonResult> UpdateSetting(List<PaymentGatewaySetting> settings)
        {
            try
            {
                await _paymentService.UpdateGatewaySetting(settings);
                _notificationService.Notify(_localeResourceProvider.Get("Success"),
                    _localeResourceProvider.Get("Data has been saved successfully."), NotificationType.Success);

                return Json(new
                {
                    Code = 200,
                    Data = true,
                    Message = ""
                }
                    );
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Code = 400,
                    Message = ex.Message
                }
                    );
            }

        }

        [HttpPost]
        public async Task<JsonResult> GetSetting(PaymentGatewaySetting setting)
        {
            try
            {
                var data = await _paymentService.GetSettings(setting.SystemName);
                return Json(new
                {
                    Code = 200,
                    Data = data,
                    Message = ""
                }
                    );
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Code = 400,
                    Message = ex.Message
                }
                    );
            }

        }

        [HttpPost]
        public async Task<object> Install(PgViewModel model)
        {
            var paymentGateways = _plugins.Where(e => e.Configuration.PluginType == PluginType.PaymentGateway).ToList();
            var pg = paymentGateways.SingleOrDefault(e => e.SystemName == model.SystemName);
            bool status = false;
            if (pg != null)
            {
                status = await pg.Install();
                if(status)
                   _notificationService.Notify(_localeResourceProvider.Get("Success"),
                    _localeResourceProvider.Get("PaymentGateway.InstallMessage"), NotificationType.Success);
                else _notificationService.Notify(_localeResourceProvider.Get("Error"),
                    _localeResourceProvider.Get("PaymentGateway.FailedToInstallMessage"), NotificationType.Error);
            }
            return new
            {
                Data = new { Code = 200, Data = status, Message = "" }
            };
            // return new { Code = 200, Data = status, Message = "Payment Gateway installed successfully!" };
        }
        [HttpPost]
        public async Task<object> UnInstall(PgViewModel model)
        {
            var paymentGateways = _plugins.Where(e => e.Configuration.PluginType == PluginType.PaymentGateway).ToList();
            var pg = paymentGateways.SingleOrDefault(e => e.SystemName == model.SystemName);
            bool status = false;
            if (pg != null)
            {
                status = await pg.UnInstall();
                if(status)
                    _notificationService.Notify(_localeResourceProvider.Get("Success"),
                        _localeResourceProvider.Get("PaymentGateway.UnInstallMessage"), NotificationType.Success);
                else _notificationService.Notify(_localeResourceProvider.Get("Error"),
                    _localeResourceProvider.Get("PaymentGateway.FailedToUnInstallMessage"), NotificationType.Error);
            }
            return new
            {
                Data = new { Code = 200, Data = status, Message = _localeResourceProvider.Get("PaymentGateway.UnInstallMessage") }
            };
        }

        public class PgViewModel
        {
            public string SystemName { get; set; }

        }

    }
}