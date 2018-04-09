using System;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Kachuwa.RTC.Hubs
{
    public class NotificationHub:Hub
    {
        private readonly IRTCConnectionManager _connectionManager;


        public NotificationHub(IRTCConnectionManager connectionManager)
        {
            _connectionManager = connectionManager;
        }

        public Task Notify(string message)
        {
            return Clients.All.SendAsync("OnNotificationRecieved", message);
        }
        public override Task OnConnectedAsync()
        {
            string connectionId=Context.ConnectionId;
            _connectionManager.AddUser(connectionId);
            return base.OnConnectedAsync();
        }
        
        public override Task OnDisconnectedAsync(Exception exception)
        {
            _connectionManager.RemoveUser(Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }
        
    }
}
