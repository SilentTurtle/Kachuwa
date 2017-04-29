using System.Threading.Tasks;
using Kachuwa.Web.API;
using Microsoft.AspNetCore.Mvc;

namespace Kachuwa.ContactUs.Api
{
    [Route("api/v1/contactus")]
    public class ContactUsApiController:BaseApiController
    {
        public IContactUsService ContactUsService { get; }

        public ContactUsApiController(IContactUsService contactUsService)
        {
            ContactUsService = contactUsService;
        }

        [HttpGet]
       
        public async Task <ApiResponse> GetAllContacts()
        {
            var data=await ContactUsService.GetAll();
            return HttpResponse(200, "", data);
        }
        [HttpPost]
        [Route("save")]
        public async Task<ApiResponse> Save(ContactUsInfo model)
        {
            var data = await ContactUsService.AddContactUs(model);
            return HttpResponse(200, "", data);
        }


    }
}