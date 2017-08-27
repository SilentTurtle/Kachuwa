namespace Kachuwa.Web.Notification
{
    public class NotificationBarConfig : INotificationBarConfig
    {
        public bool AutoClose { get; set; } = false;

        public string SuccessTemplate { get; set; } = "<div class=\"notification green\"> " +
                                                      "<div class=\"notification-body\">" +
                                                      "<div class=\"mid\">" +
                                                      "<h3>{{Title}}</h3>" +
                                                      "<p>{{Message}}</p>" +
                                                      "<button class=\"notification-btn\">OK</button>" +
                                                      //"<button class=\"notification-btn\">CANCEL</button>" +
                                                      "</div></div></div>";
        public string ErrorTemplate { get; set; } = "<div class=\"notification red\"> " +
                                                    "<div class=\"notification-body\">" +
                                                    "<div class=\"mid\">" +
                                                    "<h3>{{Title}}</h3>" +
                                                    "<p>{{Message}}</p>" +
                                                    "<button class=\"notification-btn\">OK</button>" +
                                                    //"<button class=\"notification-btn\">CANCEL</button>" +
                                                    "</div></div></div>";
        public string WarningTemplate { get; set; } = "<div class=\"notification orange\"> " +
                                                      "<div class=\"notification-body\">" +
                                                      "<div class=\"mid\">" +
                                                      "<h3>{{Title}}</h3>" +
                                                      "<p>{{Message}}</p>" +
                                                      "<button class=\"notification-btn\">OK</button>" +
                                                      //"<button class=\"notification-btn\">CANCEL</button>" +
                                                      "</div></div></div>";
        public string InfoTemplate { get; set; } = "<div class=\"notification blue\"> " +
                                                   "<div class=\"notification-body\">" +
                                                   "<div class=\"mid\">" +
                                                   "<h3>{{Title}}</h3>" +
                                                   "<p>{{Message}}</p>" +
                                                   "<button class=\"notification-btn\">OK</button>" +
                                                   //"<button class=\"notification-btn\">CANCEL</button>" +
                                                   "</div></div></div>";
        public string BroadCast { get; set; } = "";

    }
}