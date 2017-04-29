using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kachuwa.ContactUs
{
    public interface IContactUsService
    {

        Task<bool> AddContactUs(ContactUsInfo model);
        Task<IEnumerable<ContactUsInfo>> GetAll();

    }
}