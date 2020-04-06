using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kachuwa.Localization;
using Kachuwa.Plugin;
using Kachuwa.Web;
using Kachuwa.Web.Model;
using Kachuwa.Web.Notification;
using Kachuwa.Web.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace Kachuwa.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(PolicyConstants.PagePermission)]
    public class PluginsController : BaseController
    {
        private readonly IPluginService _pluginService;
        private readonly IApplicationLifetime _applicationLifetime;
        private readonly IPluginProvider _pluginProvider;
        private readonly INotificationService _notificationService;
        private readonly ILocaleResourceProvider _localeResourceProvider;
        private readonly IEnumerable<IPlugin> _plugins;

        public PluginsController(IPluginService pluginService, IApplicationLifetime applicationLifetime,
           IPluginProvider pluginProvider,
           INotificationService notificationService, ILocaleResourceProvider localeResourceProvider)
        {
            _pluginService = pluginService;
            _applicationLifetime = applicationLifetime;
            _pluginProvider = pluginProvider;
            _notificationService = notificationService;
            _localeResourceProvider = localeResourceProvider;


            _plugins = _pluginProvider.Plugins;
            _localeResourceProvider.LookUpGroupAt("Plugin");
        }

        [Route("admin/plugins")]
        public async Task<ActionResult> Index()
        {
            if (_plugins != null)
            {
                var allplugins =
                    _plugins.ToList();

               var _installedGateways = await _pluginService.PluginCrudService.GetListAsync();

               var plugins = allplugins.Select(e =>
                {
                    if (_installedGateways.Any(x => x.SystemName == e.SystemName && x.IsActive == true))
                    {
                        e.Configuration.IsInstalled = true;

                        return e;
                    }
                    else
                    {
                        return e;
                    }
                }).ToList();

                return View(plugins);
            }
            return View(new List<IPlugin>());
        }

       

        public async Task<ActionResult> New()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> New(PluginViewModel model)
        {
            if (model.PluginZipFile != null)
            {
                var status = await _pluginService.UnzipAndInstall(model.PluginZipFile);
                if (status.IsInstalled)
                {
                    _notificationService.Notify(_localeResourceProvider.Get("Success"),
                        _localeResourceProvider.Get("Plugin.InstallMessage"), NotificationType.Success);
                    _notificationService.Notify(_localeResourceProvider.Get("Info"),
                        _localeResourceProvider.Get("Plugin.ConfiguringMessage"), NotificationType.Info);

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
                _notificationService.Notify(_localeResourceProvider.Get("Alert"), _localeResourceProvider.Get("Plugin.UploadZipFileFirst"),
                    NotificationType.Warning);
                ModelState.AddModelError("PluginZipFile", "Please upload file first.");
            }

            return View();
        }

        [HttpPost]
        public async Task<object> Install(PluginViewModel model)
        {
            var plugins = _plugins.ToList();
            var plugin = plugins.SingleOrDefault(e => e.SystemName == model.SystemName);
            bool status = false;
            if (plugin != null)
            {
                status = await plugin.Install();
                if (status)
                    _notificationService.Notify(_localeResourceProvider.Get("Success"),
                     _localeResourceProvider.Get("Plugin.InstallMessage"), NotificationType.Success);
                else _notificationService.Notify(_localeResourceProvider.Get("Error"),
                    _localeResourceProvider.Get("Plugin.FailedToInstallMessage"), NotificationType.Error);
            }
            return new
            {
                Data = new { Code = 200, Data = status, Message = "" }
            };
            // return new { Code = 200, Data = status, Message = "Payment Gateway installed successfully!" };
        }
        [HttpPost]
        public async Task<object> UnInstall(PluginViewModel model)
        {
            var plugins = _plugins.ToList();
            var plugin = plugins.SingleOrDefault(e => e.SystemName == model.SystemName);
            bool status = false;
            if (plugin != null)
            {
                status = await plugin.UnInstall();
                if (status)
                    _notificationService.Notify(_localeResourceProvider.Get("Success"),
                        _localeResourceProvider.Get("Plugin.UnInstallMessage"), NotificationType.Success);
                else _notificationService.Notify(_localeResourceProvider.Get("Error"),
                    _localeResourceProvider.Get("Plugin.FailedToUnInstallMessage"), NotificationType.Error);
            }
            return new
            {
                Data = new { Code = 200, Data = status, Message = _localeResourceProvider.Get("PaymentGateway.UnInstallMessage") }
            };
        }

        public async Task<IActionResult> UpdateUsage(Plugin.Plugin plugin)
        {
            try
            {
                var data = await _pluginService.UpdateStatus(plugin);
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

    }


}