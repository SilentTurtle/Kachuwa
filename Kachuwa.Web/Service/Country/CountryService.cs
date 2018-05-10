using Kachuwa.Data;
using Kachuwa.Web.Model;

namespace Kachuwa.Web.Services
{
    public class CountryService : ICountryService {
        public CrudService<Country> CountryCrudService { get; set; }=new CrudService<Country>();
    }
}