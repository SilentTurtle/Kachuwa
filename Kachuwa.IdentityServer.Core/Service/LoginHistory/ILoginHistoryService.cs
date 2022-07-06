
using Kachuwa.Data;

namespace Kachuwa.IdentityServer.Service
{
    public interface ILoginHistoryService
    {
        CrudService<UserLoginHistory> HistoryService { get; set; }
        Task<UserLoginHistory> GetLastLoginInfoAsync(long userId);
        Task<bool> RemoveDeviceAsync(string deviceIdentifier,long userId);
        Task<bool> CheckLoginDeviceAsync(string deviceIdentifier, long userId);
        Task<int> GetUserLoggedInDevicesNumber(long userId);

    }
}