namespace Kachuwa.Web
{
    public class NotificationHandler : WebSocketHandler
    {
        public NotificationHandler(WebSocketConnectionManager webSocketConnectionManager) : base(webSocketConnectionManager)
        {
        }
    }

    public class ApplicationInsightHandler : WebSocketHandler
    {
        public ApplicationInsightHandler(WebSocketConnectionManager webSocketConnectionManager) : base(webSocketConnectionManager)
        {

        }
    }
}