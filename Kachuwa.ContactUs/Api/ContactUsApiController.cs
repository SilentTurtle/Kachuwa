using System.Threading.Tasks;
using Kachuwa.Data.Crud.FormBuilder;
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
            var data=await ContactUsService.ContactCrudService.GetListAsync();
            return HttpResponse(200, "", data);
        }
        [HttpPost]
        [Route("save")]
        public async Task<ApiResponse> Save(ContactUsInfo model)
        {
            if (ModelState.IsValid)
            {
                model.AutoFill();
                var data = await ContactUsService.ContactCrudService.InsertAsync<int>(model);
                return HttpResponse(200, "", data);
            }
            else
            {
                return HttpResponse(501, "", "validation error");
            }
        }


    }
}