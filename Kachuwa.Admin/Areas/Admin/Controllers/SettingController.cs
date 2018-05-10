using System;
using System.Linq;
using System.Threading.Tasks;
using Kachuwa.Caching;
using Kachuwa.Web;
using Kachuwa.Web.Model;
using Kachuwa.Web.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Kachuwa.Core.DI;
using Kachuwa.Data.Extension;
using Kachuwa.Extensions;
using Kachuwa.Web.Notification;
using Kachuwa.Web.Security;
using Kachuwa.Web.Services;
using Microsoft.AspNetCore.Hosting;

namespace Kachuwa.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(PolicyConstants.PagePermission)]
    public class SettingController : BaseController
    {
        private readonly ISettingService _settingService;
        private readonly IApplicationLifetime _applicationLifetime;
        private readonly IFileService _fileService;
        private readonly IServiceCollection _diService;
        private readonly INotificationService _notificationService;

        public SettingController(ISettingService settingService, IApplicationLifetime applicationLifetime,
            IFileService fileService,
            IServiceCollection diService,INotificationService notificationService)
        {
            _settingService = settingService;
            _applicationLifetime = applicationLifetime;
            _fileService = fileService;

            _diService = diService;
            _notificationService = notificationService;
        }
        public async Task<IActionResult> Index()
        {
            return RedirectToAction("Web");
        }
        public async Task<IActionResult> Security()
        {
            
            return View();
        }

        public async Task<string> Shutdown()
        {
            // Later bro
            _applicationLifetime.StopApplication();
            return "Ok";
        }
        public async Task<IActionResult> Web()
        {
            var _setting =await  _settingService.CrudService.GetAsync(1);
            return View(_setting);
        }
        [HttpPost]
        public async Task<IActionResult> Web(Setting model)
        {
            if (ModelState.IsValid)
            {
                model.AutoFill();
                model.Description.Trim();
                if (model.LogoFile != null)
                {
                    model.Logo = _fileService.Save("Logo", model.LogoFile);
                }
                await _settingService.CrudService.UpdateAsync(model);
                _diService.TryAddSingleton(model.To<Setting>());
                _notificationService.Notify("Success", "Data has been saved successfully!", NotificationType.Success);
                // _diService.BuildServiceProvider(true);
                // var asdf= serviceProvider.GetService<Setting>();
                //_diService.TryAddSingleton(model);
                //_diService.BuildServiceProvider();

            }
            else
            {
                _notificationService.Notify("Warning", "Please enter valid values!", NotificationType.Warning);
                ModelState.AddModelError("Invalid Setting Values","Please enter valid values");
                return View(model);
            }
        
            return View(model);
        }
        public async Task<IActionResult> Caching()
        {
            return View();
        }
    }
}