using System.Threading.Tasks;
using Kachuwa.Web.API;
using Microsoft.AspNetCore.Mvc;

namespace Kachuwa.Web
{
    [Route("api/v1/notification")]
    public class NotificationApiController : BaseApiController
    {
        private NotificationHandler NotificationsMessageHandler { get; set; }

        public NotificationApiController(NotificationHandler notificationsMessageHandler)
        {
            NotificationsMessageHandler = notificationsMessageHandler;
        }

        [HttpGet]
        public async Task Notify([FromQuery]string message)
        {
            await NotificationsMessageHandler.InvokeClientMethodToAllAsync("receiveMessage", message);
        }
       [HttpPost]
        public async Task NotifyTo(NotifyUserModel model)
        {

            await NotificationsMessageHandler.InvokeClientMethodAsync(model.SocketId, "receiveMessage", model.Message);
        }

        public class NotifyUserModel
        {
            public object[] Message;
            public string SocketId { get; set; }
        }
    }
   
}