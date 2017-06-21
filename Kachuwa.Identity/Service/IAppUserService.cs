using System.Threading.Tasks;
using Kachuwa.Data;
using Kachuwa.Identity.Models;

namespace Kachuwa.Identity.Service
{
    public interface IAppUserService
    {
        CrudService<AppUser> AppUserCrudService { get; set; }
        Task<bool> UpdateDeleteAsync(int id);
    }
}