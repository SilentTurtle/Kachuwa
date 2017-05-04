using System.Threading.Tasks;
using Kachuwa.Data;

namespace Kachuwa.Web
{
    public interface IPageService
    {
        CrudService<Page> CrudService { get; set; }
        Task<bool> CheckPageExist(string url);
    }
}