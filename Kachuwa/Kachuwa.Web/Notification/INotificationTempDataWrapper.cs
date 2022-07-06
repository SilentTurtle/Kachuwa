namespace Kachuwa.Web.Notification
{
    public interface INotificationTempDataWrapper
    {
        T Get<T>(string key);
        T Peek<T>(string key);
        void Add(string key, object value);
    }
}