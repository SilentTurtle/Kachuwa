using System.Collections.Generic;
using System.Threading.Tasks;
using Kachuwa.Web;
using Kachuwa.Web.Notification;
using Microsoft.AspNetCore.SignalR;
using Kachuwa.Extensions;
using Kachuwa.Identity.Extensions;
using System.Linq;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace Kachuwa.RTC.Hubs
{

    //public static class HubExtensions
    //{
    //    public static string[] GetUserConnectionId<T>(this IHubContext<T> hubContext) where T:BaseHub
    //    {
    //        var baseHub = (BaseHub) hubContext;
    //        var connectionIds = baseHub.ConnectionManager.GetUserConnectionIds(ContextResolver.Context.User.Identity.GetIdentityUserId()).Result;
    //        return connectionIds?.ToArray();
    //    }
    //}
   // [Authorize(JwtBearerDefaults.AuthenticationScheme)]
    public class KachuwaUserHub : BaseHub
    {
        public KachuwaUserHub(IRTCConnectionManager connectionManager) : base(connectionManager)
        {
        }
        public Task BroadcastMessage(Notification notification)
        {
            //TODO lock persistantly
            return Clients.All.SendAsync("OnBroadcastRecieve", notification);
            // return Clients.All.BroadcastMessage(notification);
        }
        public async Task NotityMe(long userId, Notification notification)
        {
            var selfConnection = Context.ConnectionId;
            var connectionIds = await ConnectionManager.GetUserConnectionIds(userId);
            await Clients.Users((IReadOnlyList<string>)connectionIds).SendAsync("OnNotificationRecieved", notification);
            //return Clients.User(userId).BroadcastMessage(notification);

        }
        public async Task NotityUser(long userId, Notification notification)
        {

            var connectionIds = await ConnectionManager.GetUserConnectionIds(userId);
            await Clients.Users((IReadOnlyList<string>)connectionIds).SendAsync("OnNotificationRecieved", notification);
            //return Clients.User(userId).BroadcastMessage(notification);

        }
        public async Task NotifyGroup(string group, Notification notification)
        {
            // var connectionIds = await ConnectionManager.GetUserConnectionIdsByGroup(group);
            await Clients.Group(group).SendAsync("OnNotificationRecieved", notification);
            //return Clients.User(userId).BroadcastMessage(notification);

        }
        public async Task NofifyToRole(string roleName, Notification notification)
        {
            var connectionIds = await ConnectionManager.GetUserConnectionIdsByRoles(roleName);
            await Clients.Users((IReadOnlyList<string>)connectionIds).SendAsync("OnNotificationRecieved", notification);
            // return Clients.All.onNofify(notification);
        }
    }
    public class RTCNavigationHub : BaseHub
    {
        public RTCNavigationHub(IRTCConnectionManager connectionManager) : base(connectionManager)
        {
        }
        public Task Navigating(long userId,string lu,string cu)
        {
          
            return Clients.All.SendAsync("OnUserNavigating", new{lu,cu});
            // return Clients.All.BroadcastMessage(notification);
        }
       
    }


   
}