namespace Kachuwa.Web.Notification
{
    public interface INotificationBarConfig
    {
        bool AutoClose { get; set; }
        string SuccessTemplate { get; set; }
        string ErrorTemplate { get; set; }
        string WarningTemplate { get; set; }
        string InfoTemplate { get; set; }
        string BroadCast { get; set; }

    }
}