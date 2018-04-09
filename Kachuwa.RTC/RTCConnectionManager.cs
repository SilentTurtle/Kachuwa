using System;
using System.Collections.Concurrent;
using Kachuwa.Web;
using Kachuwa.Identity.Extensions;
using System.Linq;

namespace Kachuwa.RTC
{
    public interface IRTCConnectionManager
    {
        ConcurrentDictionary<string, RTCUser> RealWebUsers { get; set; }
        ConcurrentDictionary<string, RTCUser> GetAll();
        string GetId(RTCUser user);
        bool AddUser(string connectionId);
        bool RemoveUser(string connectionId);
    }
    public class RTCConnectionManager: IRTCConnectionManager
    {
        public ConcurrentDictionary<string, RTCUser> RealWebUsers { get; set; } = new ConcurrentDictionary<string, RTCUser>();

        public RTCUser GetWebUserById(string connectionId)
        {
            return RealWebUsers.FirstOrDefault(p => p.Key == connectionId).Value;
        }
      

        public ConcurrentDictionary<string, RTCUser> GetAll()
        {
            return RealWebUsers;
        }

        public string GetId(RTCUser user)
        {
            return RealWebUsers.FirstOrDefault(p => p.Value == user).Key;
        }

        private bool CheckExistingConnectionUser(string connectionId)
        {
            var context = ContextResolver.Context;
            //does'nt exist

            //for logged in user
            if (context.User.Identity.IsAuthenticated)
            {
                string userId = context.User.Identity.GetIdentityUserId().ToString();
                if (!RealWebUsers.ContainsKey(userId))
                {

                    //session exist
                    var existingUser = RealWebUsers.FirstOrDefault(p => p.Key == userId);
                    if (existingUser.Key != null)
                    {
                        //adding new connectionid
                        existingUser.Value.ConnectionIds.Add(connectionId);
                        return true;
                    }
                    else
                    {
                        // if session does not exist add
                        return false;
                    }
                }
            }
            else
            {//guest user tracking from sessions
                string userId = context.Session.Id;
                if (!RealWebUsers.ContainsKey(userId))
                {

                    //session exist
                    var existingUser = RealWebUsers.FirstOrDefault(p => p.Key == userId);
                    if (existingUser.Key != null)
                    {
                        //adding new connectionid
                        existingUser.Value.ConnectionIds.Add(connectionId);
                        return true;
                    }
                    else
                    {
                        // if session does not exist add
                        return false;
                    }
                }
            }
            return false;
        }
        public bool AddUser(string connectionId)
        {
            //string connectionId = CreateConnectionId();
            if (!CheckExistingConnectionUser(connectionId))
            {
                var rtcUser = new RTCUser();
                var context = ContextResolver.Context;
                string ua = context.Request.Headers["User-Agent"].ToString();
                rtcUser.UserDevice = ua;//new UserAgent(ua).Browser.ToString();
                rtcUser.ConnectionIds.Add(connectionId);
                rtcUser.IdentityUserId = context.User.Identity.GetIdentityUserId();
                rtcUser.SessionId = context.Session.Id;
                RealWebUsers.TryAdd(
                    context.User.Identity.IsAuthenticated
                        ? context.User.Identity.GetIdentityUserId().ToString()
                        : context.Session.Id, rtcUser);
                return true;
            }
            return false;
        }

        public bool RemoveUser(string connectionId)
        {
            RTCUser user;
            var context = ContextResolver.Context;
            RealWebUsers.TryGetValue(context.User.Identity.IsAuthenticated
                ? context.User.Identity.GetIdentityUserId().ToString()
                : context.Session.Id, out user);
            if (user != null)
            {
                user.ConnectionIds.Remove(connectionId);
                if (user.ConnectionIds.Count == 0)
                {
                    RealWebUsers.TryRemove(context.User.Identity.IsAuthenticated
                        ? context.User.Identity.GetIdentityUserId().ToString()
                        : context.Session.Id, out user);
                }
                return true;
            }


            return false;
        }

        private string CreateConnectionId()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
