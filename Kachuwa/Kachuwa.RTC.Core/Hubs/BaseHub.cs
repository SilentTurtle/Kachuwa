using System;
using System.Buffers;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Kachuwa.Identity.Extensions;
using Kachuwa.Web;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Http.Connections.Features;
using Microsoft.AspNetCore.SignalR;

namespace Kachuwa.RTC.Hubs
{    

    //[Authorize(JwtBearerDefaults.AuthenticationScheme)]
    //[Authorize]
    public class BaseHub : Hub
    {
        public readonly IRTCConnectionManager ConnectionManager;
        
        public BaseHub(IRTCConnectionManager connectionManager)
        {
            ConnectionManager = connectionManager;
        }

        public   override Task OnConnectedAsync()
        {
            string connectionId = Context.ConnectionId;
            var rtcUser = new RTCUser();
            var context = ContextResolver.Context;
            string ua = context.Request.Headers["User-Agent"].ToString();
            rtcUser.UserDevice = ua;
            rtcUser.ConnectionIds.Add(connectionId);
            rtcUser.IdentityUserId = context.User.Identity.GetIdentityUserId();
            rtcUser.SessionId = context.Session.Id;
          
            rtcUser.HubNames.Add(this.GetType().Name);
            //rtcUser.IsFromMobile = false;
            rtcUser.IsFromWeb = true;
            rtcUser.UserRoles = string.Join(',', context.User.Identity.GetRoles());
            rtcUser.ConnectionId = connectionId;
            ConnectionManager.AddUser(rtcUser);
          
                try
                 {
                    var status =  ConnectionManager.GetOnlineUserStatus().Result;
                     Clients.All.SendAsync("OnUserChange", status);
                }
                catch (Exception e)
                {
                    
                }
               
            
         
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            ConnectionManager.RemoveUser(Context.ConnectionId);
            try
            {
                var status = ConnectionManager.GetOnlineUserStatus().Result;
                Clients.All.SendAsync("OnUserChange", status);
            }
            catch (Exception e)
            {

            }

            return base.OnDisconnectedAsync(exception);
        }
    }
}