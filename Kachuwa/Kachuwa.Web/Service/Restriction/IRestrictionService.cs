using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kachuwa.Data;
using Kachuwa.Web.Model;

namespace Kachuwa.Web.Service
{
   public interface IRestrictionService
    {
        CrudService<RestrictionKey> KeyCrudService { get; set; }
        CrudService<Restriction> RestrictionCrudService { get; set; }
        CrudService<AdministrativeIPAccess> AdminIPAccessCrudService { get; set; }
    }

   public class RestrictionService : IRestrictionService
   {
       public CrudService<RestrictionKey> KeyCrudService { get; set; } = new CrudService<RestrictionKey>();
        public CrudService<Restriction> RestrictionCrudService { get; set; } = new CrudService<Restriction>();

        public CrudService<AdministrativeIPAccess> AdminIPAccessCrudService { get; set; } =
            new CrudService<AdministrativeIPAccess>();
    }
}
