using System.Threading.Tasks;
using Kachuwa.Data;
using Kachuwa.Identity.Models;
using Microsoft.AspNetCore.Mvc.TagHelpers.Cache;

namespace Kachuwa.Identity.Service
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