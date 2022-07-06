using System;
using System.Collections.Generic;
using System.Text;
using Kachuwa.Data;
using Kachuwa.Web.Model;

namespace Kachuwa.Web.Services
{
    public interface ICountryService
    {
        CrudService<Country> CountryCrudService { get; set; }
    }
}
