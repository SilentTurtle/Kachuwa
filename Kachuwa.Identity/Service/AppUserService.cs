using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Kachuwa.Data;
using Kachuwa.Identity.Models;

namespace Kachuwa.Identity.Service
{
    public class AppUserService:IAppUserService
    {
        public CrudService<AppUser> AppUserCrudService { get; set; }=new CrudService<AppUser>();
        public async Task<bool> UpdateDeleteAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
