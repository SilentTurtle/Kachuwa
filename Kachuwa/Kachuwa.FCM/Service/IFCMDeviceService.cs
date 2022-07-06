using Kachuwa.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kachuwa.FCM
{
    public interface IFCMDeviceService
    {
        CrudService<UserFCMDevice> FCMDeviceCrudService { get; set; }

        Task AddUserFcmDevice(UserFCMDevice userFCMDevice);

        Task<List<string>> GetFcmTokenByUserId(long userId);

        Task UpdateFCMGroupbyUserId(long userId, string groupName);

        Task<IEnumerable<string>> GetFcmTokensByUserIds(string userIds);
    }
       
}