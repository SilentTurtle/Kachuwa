using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Kachuwa.Data;
using Kachuwa.Identity.Models;
using Kachuwa.Identity.ViewModels;

namespace Kachuwa.Identity.Service
{
    public interface IAppUserService
    {
        CrudService<AppUser> AppUserCrudService { get; set; }
        CrudService<UserType> UserTypeCrudService { get; set; }
        Task<UserStatus> SaveNewUserAsync(UserViewModel model);
        Task<UserStatus> UpdateUserAsync(UserEditViewModel model);
        Task<bool> DeleteUserAsync(int appUserId);
        Task<bool> AssignRolesAsync(UserRolesViewModel roles);

        Task<UserEditViewModel> GetAsync(int appuserId);
        Task<bool> UpdateProfilePicture(long appUserId, string imagePath);
        Task<bool> UpdateUserDeviceId(long appUserId, string deviceId);

        Task<int> GetNewUserStatusAsync(int showRecords);
        Task<UserStatus> AddExternalUser(string provider, string userId, List<Claim> claims);
        Task<IEnumerable<UserExportViewModel>> ExportAllUsersAsync(int organizationId);
        Task<IEnumerable<UserExportViewModel>> ExportAllPackageUsersAsync(int organizationId,int packageId);
        Task<IEnumerable<UserPrintouViewModel>> GetAllPackageUser(int organizationId, int packageId);
        Task<string> GetUserEmailById(long customerId);
        Task<bool> ChangeEmailAsync(long userId, string email);
        Task<bool> ChangePhoneNumberAsync(long userId, string phoneNumber);
        Task<bool> CheckEmailExists(string email);
        Task<IEnumerable<AppUser>> GetUsersByRoles(string role, int pageNo, int limit, string query);
        Task<bool> CheckPhoneNumberExists(string phoneNumber);
    }
}