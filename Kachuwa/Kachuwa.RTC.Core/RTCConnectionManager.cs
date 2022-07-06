using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Kachuwa.Web;
using Kachuwa.Identity.Extensions;
using System.Linq;
using System.Threading.Tasks;

namespace Kachuwa.RTC
{
    public class MemoryConnectionManager : IRTCConnectionManager
    {
        public static ConcurrentDictionary<string, RTCUser> RealWebUsers { get; set; } = new ConcurrentDictionary<string, RTCUser>();

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
                var userId = context.User.Identity.GetIdentityUserId();
                if (RealWebUsers.Any(x=>x.Value.IdentityUserId== userId))
                {

                    //session exist
                    var existingUser = RealWebUsers.FirstOrDefault(p => p.Value.IdentityUserId == userId);
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
                string sessionId = context.Session.Id;
                if (RealWebUsers.Any(x => x.Value.SessionId == sessionId))
                {

                    //session exist
                    var existingUser = RealWebUsers.FirstOrDefault(x => x.Value.SessionId == sessionId);
                    if (existingUser.Value != null)
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

        private bool UpdateHubNamsOfExisting(string connectionId,string hubName)
        {
            var context = ContextResolver.Context;
            //does'nt exist

            //for logged in user
            if (context.User.Identity.IsAuthenticated)
            {
                var userId = context.User.Identity.GetIdentityUserId();
                if (RealWebUsers.Any(x => x.Value.IdentityUserId == userId))
                {

                    //session exist
                    var existingUser = RealWebUsers.FirstOrDefault(p => p.Value.IdentityUserId == userId);
                    if (existingUser.Key != null)
                    {
                        //adding new connectionid
                        if (!existingUser.Value.HubNames.Contains(hubName) && !string.IsNullOrEmpty(hubName))
                            existingUser.Value.HubNames.Add(hubName);
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
                string sessionId = context.Session.Id;
                if (RealWebUsers.Any(x => x.Value.SessionId == sessionId))
                {

                    //session exist
                    var existingUser = RealWebUsers.FirstOrDefault(x => x.Value.SessionId == sessionId);
                    if (existingUser.Value != null)
                    {
                        //adding new connectionid
                        if (!existingUser.Value.HubNames.Contains(hubName) && !string.IsNullOrEmpty(hubName))
                            existingUser.Value.HubNames.Add(hubName);
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

        public bool AddUser(string connectionId,RTCUser rtcUser)
        {
            //string connectionId = CreateConnectionId();
          
            if (!CheckExistingConnectionUser(connectionId))
            {
                var context = ContextResolver.Context;
                RealWebUsers.TryAdd(
                    context.User.Identity.IsAuthenticated
                        ? context.User.Identity.GetIdentityUserId().ToString()
                        : context.Session.Id, rtcUser);
                return true;
            }
            else
            {
                UpdateHubNamsOfExisting(connectionId, rtcUser.HubNames[0]);
            }

            return false;
        }

        public bool AddUser(RTCUser rtcUser)
        {
            return AddUser(rtcUser.ConnectionId, rtcUser);
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
        public Task<RTCUser> GetCurrentUser(string connectionId)
        {
            RTCUser user;
            var context = ContextResolver.Context;
            RealWebUsers.TryGetValue(context.User.Identity.IsAuthenticated
                ? context.User.Identity.GetIdentityUserId().ToString()
                : context.Session.Id, out user);
            //setting looking connectionid as main
            user.ConnectionId = connectionId;
            return Task.FromResult(user);
        }

        public Task<RTCUser> UpdateGroupName(string groupName, long identityUserId)
        {
            RTCUser user;
            var context = ContextResolver.Context;
            RealWebUsers.TryGetValue(context.User.Identity.GetIdentityUserId().ToString(),out user);
            if (user != null)
            {
                user.GroupName=groupName;
                if (!user.GroupNames.Contains(groupName) && !string.IsNullOrEmpty(groupName))
                    user.GroupNames.Add(groupName);

                return Task.FromResult(user);
            }
            return null;
        }

        public Task<RTCUser> UpdateGroupName(string groupName, string connectionId)
        {
            RTCUser user;
            var context = ContextResolver.Context;
            RealWebUsers.TryGetValue(context.Session.Id, out user);
            if (user != null)
            {
                user.GroupName = groupName;
                if (!user.GroupNames.Contains(groupName) && !string.IsNullOrEmpty(groupName))
                    user.GroupNames.Add(groupName);

                return Task.FromResult(user);
            }
            return null;
        }

        public async Task<IEnumerable<string>> GetUserConnectionIds(long identityUserId)
        {
            return RealWebUsers.Where(x => x.Value.IdentityUserId == identityUserId).Select(y => y.Key).ToList();
        }

        public Task<int> GetOnlineUserByHub(string hubName)
        {
            throw new NotImplementedException();
        }

        public async Task<int> GetOnlineUser()
        {
            return RealWebUsers.Count;

        }

        public Task<IEnumerable<string>> GetUserConnectionIdsByRoles(string role)
        {
            throw new NotImplementedException();

        }

        public async Task<RtcUserStatus> GetOnlineUserStatus()
        {
            var loggedinUser = RealWebUsers.Select(x => x.Value.IdentityUserId!=0).Distinct().Count();
            var totalGuestUser = RealWebUsers.Select(x => x.Value.SessionId).Distinct().Count();
            var devices = RealWebUsers.Select(x => x.Value.UserDevice).Select(z => new UserAgent(z)).ToList();
            var totalWEbUser = devices.Count(e => e.OS.Name.ToLower() == "windows" || e.OS.Name.ToLower() == "macos" || e.OS.Name.ToLower() == "linux" || e.OS.Name.ToLower() == "ubuntu");
            var mobileUser = devices.Count(e => e.OS.Name.ToLower() == "android" || e.OS.Name.ToLower() == "ios");


            return new RtcUserStatus
            {
                TotalGuestUser = totalGuestUser,
                TotalUser = loggedinUser,
                TotalMobileUser = mobileUser,
                TotalWebUser = totalWEbUser
            };
        }

        private string CreateConnectionId()
        {
            return Guid.NewGuid().ToString();
        }

        public async Task<List<RTCUser>> GetUsersByGroup(string groupName)
        {
           var groupUsers=  RealWebUsers.Where(x => x.Value.GroupName.Contains(groupName)).Select(y=>y.Value).ToList();
            return groupUsers;
        }

        public Task<RTCUser> GetUser(long identityUserid, string connectionId)
        {
            RTCUser user;
            if (identityUserid==0& !string.IsNullOrWhiteSpace(connectionId))
            {
              
                var context = ContextResolver.Context;
                user = RealWebUsers.SingleOrDefault(x => x.Value.ConnectionId == connectionId).Value;
                return Task.FromResult(user);
            }
            else
            {
                user = RealWebUsers.SingleOrDefault(x => x.Value.IdentityUserId == identityUserid).Value;
                return Task.FromResult(user);
            }            
            
        }

        public async Task<List<RTCUser>> GetUsersByHub(string hubName)
        {
            return RealWebUsers.Where(x => x.Value.HubNames.Contains(hubName)).Select(y => y.Value).ToList(); 
           
        }
    }
}
