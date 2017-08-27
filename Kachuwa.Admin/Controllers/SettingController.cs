using System;
using System.Linq;
using System.Threading.Tasks;
using Kachuwa.Caching;
using Kachuwa.Data.Crud.FormBuilder;
using Kachuwa.Web;
using Kachuwa.Web.Model;
using Kachuwa.Web.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Kachuwa.Core.DI;
using Kachuwa.Extensions;
using Kachuwa.Web.Notification;

namespace Kachuwa.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class SettingController : BaseController
    {
        private readonly ISettingService _settingService;
        private readonly IServiceCollection _diService;
        private readonly INotificationService _notificationService;

        public SettingController(ISettingService settingService,IServiceCollection diService,INotificationService notificationService)
        {
            _settingService = settingService;
           
            _diService = diService;
            _notificationService = notificationService;
        }
        public async Task<IActionResult> Security()
        {
            
            return View();
        }
        public async Task<IActionResult> Web()
        {
            var _setting = _diService.BuildServiceProvider().GetService<Setting>();
            return View(_setting);
        }
        [HttpPost]
        public async Task<IActionResult> Web(Setting model)
        {
            if (ModelState.IsValid)
            {
                model.AutoFill();
                model.Description.Trim();
                await _settingService.CrudService.UpdateAsync(model);
                _diService.TryAddSingleton(model.To<Setting>());
                _notificationService.Notify("Saved Successfully!", NotificationType.Error);
                // _diService.BuildServiceProvider(true);
                // var asdf= serviceProvider.GetService<Setting>();
                //_diService.TryAddSingleton(model);
                //_diService.BuildServiceProvider();

            }
            else
            {
                _notificationService.Notify("Validation Error!", NotificationType.Error);
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