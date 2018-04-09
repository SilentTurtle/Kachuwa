using Kachuwa.Web;
using Kachuwa.Web.Notification;
using Microsoft.AspNetCore.Mvc;

namespace Kachuwa.RTC
{
    public class NotificationTestController : BaseController
    {
        private readonly INotificationService _notificationService;

        public NotificationTestController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [Route("nt")]
        public JsonResult Test()
        {
            _notificationService.Notify("title", "this is meesate", NotificationType.Success);
            return Json(true);
        }
    }
}