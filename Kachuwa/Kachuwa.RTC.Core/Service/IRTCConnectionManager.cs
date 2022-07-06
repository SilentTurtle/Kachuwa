using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Kachuwa.RTC
{
    public interface IRTCConnectionManager
    {   
       // string GetId(RTCUser user);
        bool AddUser(RTCUser user);
       // bool AddUser(string connectionId);
        bool RemoveUser(string connectionId);
        Task<RTCUser> GetCurrentUser(string connectionId);
        Task<RTCUser> UpdateGroupName(string groupName, long identityUserId);
        Task<RTCUser> UpdateGroupName(string groupName, string connectionId);
        Task<IEnumerable<string>> GetUserConnectionIds(long identityUserId);
        Task<int> GetOnlineUserByHub(string hubName);
        Task<int> GetOnlineUser();
        Task<IEnumerable<string>> GetUserConnectionIdsByRoles(string role);
        Task<RtcUserStatus> GetOnlineUserStatus();
        Task<List<RTCUser>> GetUsersByGroup(string groupName);
        Task<RTCUser> GetUser(long identityUserid, string connectionId);
        Task<List<RTCUser>> GetUsersByHub(string hubName);
    }

}