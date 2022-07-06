using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Validation;
using Kachuwa.Identity.Extensions;
using Kachuwa.RTC.Hubs;
using Kachuwa.Web;
using Kachuwa.Web.Notification;
using Microsoft.AspNetCore.SignalR;

namespace Kachuwa.RTC
{
    public class RTCNotificationService : INotificationService
    {
        private readonly INotificationTempDataWrapper _tempDataWrapper;

        private readonly IHubContext<KachuwaUserHub> _hubContext;

        private readonly IRTCConnectionManager _connectionManager;
        // private readonly IHubContext<KachuwaNotificationHub, IKachuwaNotificationHubClient> _kachuwahubClientContext;

        public RTCNotificationService(INotificationTempDataWrapper tempDataWrapper,IHubContext<KachuwaUserHub> hubContext, IRTCConnectionManager connectionManager)
        {
            _tempDataWrapper = tempDataWrapper;
            _hubContext = hubContext;
            _connectionManager = connectionManager;
            // _kachuwahubClientContext = kachuwahubClientContext;
        }
        private readonly string _key = NotificationConstants.NotificationKey;

       
        private bool QueueToTempData(Notification notification)
        {
            switch (notification.NotifyTo)
            {
                case NotifyTo.AdminOnly:
                    break;
                case NotifyTo.AllUser:
                    break;
                case NotifyTo.MeOnly:
                    break;
                case NotifyTo.SpecificUsers:
                    break;
            }

            //var messages = _tempDataWrapper.Get<IList<Notification>>(_key) ?? new List<Notification>();
            //messages.Add(notification);
            //_tempDataWrapper.Add(_key, messages);

            var userConnectionIds =
                _connectionManager.GetUserConnectionIds(ContextResolver.Context.User.Identity.GetIdentityUserId()).Result;
            _hubContext.Clients.Clients(userConnectionIds.ToArray()).SendAsync("OnNotificationRecieved", notification);
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