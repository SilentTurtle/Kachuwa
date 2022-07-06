using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kachuwa.Web
{
    public interface IWidgetService
    {
        Task<bool> Load();
        Task<IWidget> Find(string name);
        Task<IEnumerable<IWidget>> GetByConfigSource(string configSourcePath);
        Task<IEnumerable<IWidget>> GetAllWidgets();


    }
}