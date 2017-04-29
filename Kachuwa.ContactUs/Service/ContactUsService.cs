using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Kachuwa.ContactUs
{
    public class ContactUsService: IContactUsService
    {
        public Task<bool> AddContactUs(ContactUsInfo model)
        {
            return Task.FromResult(true);
        }

        public Task<IEnumerable<ContactUsInfo>> GetAll()
        {
            return null;
        }
    }
}
