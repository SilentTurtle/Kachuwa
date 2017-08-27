using System.Collections.Generic;
using System.Threading.Tasks;
using Kachuwa.Data;
using Kachuwa.Identity.Models;
using Kachuwa.Identity.ViewModels;

namespace Kachuwa.Identity.Service
{
    public interface IAppUserService
    {
        CrudService<AppUser> AppUserCrudService { get; set; }
        Task<UserStatus> SaveNewUserAsync(UserViewModel model);
        Task<UserStatus> SaveUserAsync(UserEditViewModel model);
        Task<bool> DeleteUserAsync(int appUserId);
        Task<bool> AssignRolesAsync(UserRolesViewModel roles);

        Task<UserEditViewModel> GetAsync(int appuserId);
    }
}