using Kachuwa.Data;
using Kachuwa.Web.Model;
using System;
using System.Data;
using System.Threading.Tasks;

namespace Kachuwa.Web.Services
{
    public interface IScreenService
    {
        CrudService<Screen> CrudService { get; set; }

    }
    public class ScreenService : IScreenService
    {
        public CrudService<Screen> CrudService { get; set; } = new CrudService<Screen>();
    }

}