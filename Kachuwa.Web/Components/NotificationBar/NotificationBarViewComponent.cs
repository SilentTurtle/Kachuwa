using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kachuwa.Log;
using Kachuwa.Web.Module;
using Kachuwa.Web.Notification;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Mustache;

namespace Kachuwa.Web.Components
{
    [ViewComponent(Name = "NotificationBar")]
    public class NotificationBarViewComponent : KachuwaViewComponent
    {
        private readonly ILogger _logger;
        private readonly ITemplateEngine _templateEngine;
        private readonly INotificationTempDataWrapper _notificationTempDataWrapper;
        private readonly INotificationBarConfig _config;

        public NotificationBarViewComponent(ILogger logger, ITemplateEngine templateEngine, INotificationTempDataWrapper notificationTempDataWrapper,
             INotificationBarConfig config)
        {
            _logger = logger;
            _templateEngine = templateEngine;
            _notificationTempDataWrapper = notificationTempDataWrapper;
            
            _config = config;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            try
            {
                var model = new NotificationViewModel();
                model.Config = _config;
                var notifications = _notificationTempDataWrapper.Peek<IEnumerable<Notification.Notification>>(NotificationConstants.NotificationKey);
                if (notifications != null)
                {
                    foreach (var notification in notifications)
                    {
                        model.Templates.Add(Render(notification));
                    }
                }
                return View(model);
            }
            catch (Exception e)
            {
                _logger.Log(LogType.Error, () => "Notification loading error.", e);
                throw e;
            }

        }

        public HtmlString Render(Notification.Notification notification)
        {
            string rendered = "";
            if (notification.Type == NotificationType.Error)
            {
                rendered= _templateEngine.Render(_config.ErrorTemplate, notification);
            }
            if (notification.Type == NotificationType.Success)
            {
                rendered = _templateEngine.Render(_config.SuccessTemplate, notification);
            }
            if (notification.Type == NotificationType.Info)
            {
                rendered = _templateEngine.Render(_config.InfoTemplate, notification);
            }
            if (notification.Type == NotificationType.Warning)
            {
                rendered = _templateEngine.Render(_config.WarningTemplate, notification);
            }
            return new HtmlString(rendered);
        }

        public override string DisplayName { get; } = "Notification Bar";
        public override bool IsVisibleOnUI { get; } = true;
    }
}