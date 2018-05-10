using System;
using Kachuwa.Data;
using Kachuwa.Web.Model;

namespace Kachuwa.Web.Services
{
    public interface IAuditService
    {
        CrudService<Audit> CrudService { get; set; }
    }
}
