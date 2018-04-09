using System;
using System.Threading.Tasks;
using Kachuwa.Web.Notification;
using Microsoft.AspNetCore.SignalR;

namespace Kachuwa.RTC
{
    public class RTCNotificationService : INotificationService
    {
        private readonly IHubContext<KachuwaNotificationHub> _hubContext;
       // private readonly IHubContext<KachuwaNotificationHub, IKachuwaNotificationHubClient> _kachuwahubClientContext;

        public RTCNotificationService(IHubContext<KachuwaNotificationHub> hubContext
           // , IHubContext<KachuwaNotificationHub, IKachuwaNotificationHubClient> kachuwahubClientContext
            )
        {
            _hubContext = hubContext;
           // _kachuwahubClientContext = kachuwahubClientContext;
        }
        private bool QueueToTempData(Notification notification)
        {
            _hubContext.Clients.All.SendAsync("OnNotificationRecieved", notification);
            //_kachuwahubClientContext.Clients.All.onNofify(notification);
            return true;
        }

        public bool Success(Notification notification)
        {
            notification.Type = NotificationType.Success;
            return QueueToTempData(notification);
        }

        public bool Error(Notification notification)
        {
            notification.Type = NotificationType.Error;
            return QueueToTempData(notification);
        }

        public bool Info(Notification notification)
        {
            notification.Type = NotificationType.Info;
            return QueueToTempData(notification);
        }

        public bool Warning(Notification notification)
        {
            notification.Type = NotificationType.Warning;
            return QueueToTempData(notification);
        }

        public bool BroadCast(Notification notification)
        {
            _hubContext.Clients.All.SendAsync("OnBroadcastRecieved", notification);
            return true;
        }

        public bool BroadCast(string roles, Notification notification)
        {
            throw new NotImplementedException();
        }

        public bool Notify(string title, string message)
        {
            var notification = new Notification
            {
                Title = title,
                Message = message
            };
            notification.Type = NotificationType.Info;
            return QueueToTempData(notification);
        }

        public bool Notify(string title, string message, NotificationType notificationType)
        {
            var notification = new Notification
            {
                Title = title,
                Message = message
            };
            notification.Type = notificationType;
            return QueueToTempData(notification);
        }

        public bool Notify(string message, NotificationType notificationType)
        {
            var notification = new Notification
            {
                Message = message
            };
            switch (notificationType)
            {
                case NotificationType.Error:
                    notification.Title = "Error";
                    break;
                case NotificationType.Info:
                    notification.Title = "Info";
                    break;
                case NotificationType.Success:
                    notification.Title = "Success";
                    break;
                case NotificationType.Warning:
                    notification.Title = "Warning";
                    break;
            }
            notification.Type = notificationType;
            return QueueToTempData(notification);
        }
    }
}