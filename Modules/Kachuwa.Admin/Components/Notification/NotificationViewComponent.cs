using System;
using System.Threading.Tasks;
using Kachuwa.Log;
using Microsoft.AspNetCore.Mvc;

namespace Kachuwa.Admin.Components
{
    public class NotificationViewComponent : ViewComponent
    {
        private readonly ILogger _logger;

        public NotificationViewComponent(ILogger logger) 
        {
          
            _logger = logger;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            try
            {
                return View();
            }
            catch (Exception e)
            {
                _logger.Log(LogType.Error, () => "Notification loading error.", e);
                throw e;
            }
         
        }

    }
}