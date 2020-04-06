namespace Kachuwa.Web.Notification
{
    public class Notification
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public NotificationType Type { get; set; }
        public NotifyTo NotifyTo { get; set; } = NotifyTo.MeOnly;
    }

    public enum NotifyTo
    {
        MeOnly,
        AllUser,
        AdminOnly,
        SpecificUsers,
    }
}