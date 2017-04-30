using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Kachuwa.Web
{
    public class NotificationController : Controller
    {
        private NotificationHandler NotificationsMessageHandler { get; set; }

        public NotificationController(NotificationHandler notificationsMessageHandler)
        {
            NotificationsMessageHandler = notificationsMessageHandler;
        }

        [HttpGet]
        public async Task Notify([FromQuery]string message)
        {
            await NotificationsMessageHandler.InvokeClientMethodToAllAsync("receiveMessage", message);
        }
    }
   
}