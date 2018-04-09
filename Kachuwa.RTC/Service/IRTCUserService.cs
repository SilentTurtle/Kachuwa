using System.Collections.Generic;
using System.Threading.Tasks;
using Kachuwa.Data;

namespace Kachuwa.RTC
{
    public interface IRTCUserService
    {
        CrudService<RTCUser> CrudService { get; set; }

        Task UpdateGroupName(string groupName, long identityUserId);
        Task UpdateGroupName(string groupName, string connectionId);

        Task<IEnumerable<string>> GetUserConnectionIds(long identityUserId);
        Task<int> GetOnlineUserByHub(string hubName);
        Task<int> GetOnlineUser();

    }
}