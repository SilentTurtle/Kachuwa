using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Kachuwa.Data;

namespace Kachuwa.ContactUs
{
    public class ContactUsService: IContactUsService
    {
        public CrudService<ContactUsInfo> ContactCrudService { get; set; }=new CrudService<ContactUsInfo>();
    }
}
