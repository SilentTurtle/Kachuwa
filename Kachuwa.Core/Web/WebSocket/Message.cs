using System;
using Microsoft.AspNetCore.Http;

namespace Kachuwa.Web
{
    public class Message
    {
        public MessageType MessageType { get; set; }
        public string Data { get; set; }

      
    }

    public static class SocketHubs
    {
        public const string DefaultHub = "/";
        public const string NotificationHub = "/ntfy";
        public const string ChatHub = "/kcht";


        public static PathString AppInsight = "/insight";
    }

   
}