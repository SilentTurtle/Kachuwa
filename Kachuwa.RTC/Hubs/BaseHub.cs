using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Kachuwa.RTC.Hubs
{
    public class BaseHub<T> : Hub<T> where T : class
    {
        private readonly IRTCConnectionManager _connectionManager;

        public BaseHub(IRTCConnectionManager connectionManager)
        {
            _connectionManager = connectionManager;
        }

        public override Task OnConnectedAsync()
        {
            string connectionId = Context.ConnectionId;
            _connectionManager.AddUser(connectionId);
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            _connectionManager.RemoveUser(Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }
       
    }

    //[Authorize(JwtBearerDefaults.AuthenticationScheme)]
    public class BaseHub : Hub
    {
        private readonly IRTCConnectionManager _connectionManager;

        public BaseHub(IRTCConnectionManager connectionManager)
        {
            _connectionManager = connectionManager;
        }

        public override Task OnConnectedAsync()
        {
            string connectionId = Context.ConnectionId;
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