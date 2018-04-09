using System.Threading.Tasks;
using Kachuwa.RTC.Hubs;
using Kachuwa.Web.Notification;
using Microsoft.AspNetCore.SignalR;

namespace Kachuwa.RTC
{
    public interface IKachuwaNotificationHubClient
    {
        Task BroadcastMessage(Notification notification);
        Task BroadcastMessageToGroup(string groupName,Notification notification);
        Task onNofify(Notification notification);
        Task Nofify(string userName,Notification notification);
        Task NofifyToRole(string roles,Notification notification);
    }
   
    public class KachuwaNotificationHub : BaseHub
    {
        private readonly IRTCConnectionManager _connectionManager;

        public KachuwaNotificationHub(IRTCConnectionManager connectionManager) : base(connectionManager)
        {
            _connectionManager = connectionManager;
        }

        public Task BroadcastMessage(Notification notification)
        {
            //TODO lock persistantly
            return Clients.All.SendAsync("OnBroadcastRecieve", notification);
           // return Clients.All.BroadcastMessage(notification);
        }
       
        public Task InformUser(string userId, Notification notification)
        {
            //TODO lock persistantly
             return Clients.User(userId).SendAsync("OnBroadcastRecieve", notification);
            //return Clients.User(userId).BroadcastMessage(notification);

        }
        public Task Notity(Notification notification)
        {
            //TODO lock persistantly
              return Clients.All.SendAsync("OnNotificationRecieved", notification);
           // return Clients.All.onNofify(notification);
        }

    }
}