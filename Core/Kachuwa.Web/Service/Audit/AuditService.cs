using Kachuwa.Data;
using Kachuwa.Web.Model;

namespace Kachuwa.Web.Services
{
    public class AuditService : IAuditService
    {
        public CrudService<Audit> CrudService { get; set; }=new CrudService<Audit>();
    }
}