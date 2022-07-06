
using Kachuwa.Data;
using Kachuwa.IdentityServer.ViewModel;

namespace Kachuwa.IdentityServer.Service
{
    public interface IUserDeviceService
    {
        CrudService<UserDevice> DeviceService { get; set; }

        Task<UserDeviceStatus> GetUserDeviceStatus(long userId);

        Task<bool> SendDeviceVerification(long userId);
        Task<DeviceVerificationStatus> VerifyDevice(long userId, long userDeviceId);
        Task<UserDeviceStatus> CheckAndSaveDevice(UserDevice userDevice);
        Task<bool> RemoveDeviceAsync(long userDeviceId, long userId);
    }
}