namespace Kachuwa.Web.Notification
{
    public class NotificationBarConfig : INotificationBarConfig
    {
        public bool AutoClose { get; set; } = false;

        public string SuccessTemplate { get; set; } = "" +
                                                      "<div class='notification green'>" +
                                                      "<div class='dismiss'> <i class='fa fa-times-circle'></i></div>" +
                                                      "<div class='notification-body'>" +
                                                      "<div class='circle'>" +
                                                      "<i class='fa fa-2x fa-check'></i>" +
                                                      " </div>" +
                                                      "<div class='info'>" +
                                                      "<span class='notification-title'>{{Title}}</span>" +
                                                      "<span class='notification-content'>{{Message}}</span>" +
                                                      "</div> </div>";
        public string ErrorTemplate { get; set; } = 
                                                    "<div class='notification red'>" +
                                                    "<div class='dismiss'> <i class='fa fa-times-circle'></i></div>" +
                                                    "<div class='notification-body'>" +
                                                    "<div class='circle'>" +
                                                    "<i class='fa fa-2x fa-times'></i>" +
                                                    " </div>" +
                                                    "<div class='info'>" +
                                                    "<span class='notification-title'>{{Title}}</span>" +
                                                    "<span class='notification-content'>{{Message}}</span>" +
                                                    "</div> </div>";
        public string WarningTemplate { get; set; } = 
                                                      "<div class='notification yellow'>" +
                                                      "<div class='dismiss'> <i class='fa fa-times-circle'></i></div>" +
                                                      "<div class='notification-body'>" +
                                                      "<div class='circle'>" +
                                                      "<i class='fa fa-2x fa-exclamation-triangle'></i>" +
                                                      " </div>" +
                                                      "<div class='info'>" +
                                                      "<span class='notification-title'>{{Title}}</span>" +
                                                      "<span class='notification-content'>{{Message}}</span>" +
                                                      "</div> </div>";
        public string InfoTemplate { get; set; } =
                                                   "<div class='notification teal'>" +
                                                   "<div class='dismiss'> <i class='fa fa-times-circle'></i></div>" +
                                                   "<div class='notification-body'>" +
                                                   "<div class='circle'>" +
                                                   "<i class='fa fa-2x fa-fa-info'></i>" +
                                                   " </div>" +
                                                   "<div class='info'>" +
                                                   "<span class='notification-title'>{{Title}}</span>" +
                                                   "<span class='notification-content'>{{Message}}</span>" +
                                                   "</div> </div>";
        public string BroadCast { get; set; } = "";

    }
}