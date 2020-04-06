using System;
using System.Collections.Generic;
using Kachuwa.Identity.Extensions;

namespace Kachuwa.Web.Notification
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationTempDataWrapper _tempDataWrapper;
        private readonly string _key = NotificationConstants.NotificationKey;


        public NotificationService(INotificationTempDataWrapper tempDataWrapper)
        {
            _tempDataWrapper = tempDataWrapper;
        }
        private bool QueueToTempData(Notification notification)
        {
            var messages = _tempDataWrapper.Get<IList<Notification>>(_key) ?? new List<Notification>();
            messages.Add(notification);
           
            _tempDataWrapper.Add(ContextResolver.Context.User.Identity.GetIdentityUserId().ToString(), messages);
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
            throw new NotImplementedException();
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