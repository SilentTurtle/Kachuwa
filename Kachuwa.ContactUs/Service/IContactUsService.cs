using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kachuwa.Data;

namespace Kachuwa.ContactUs
{
    public interface IContactUsService
    {

        CrudService<ContactUsInfo> ContactCrudService { get; set; }

    }
}