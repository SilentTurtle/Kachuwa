using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kachuwa.Web.Layout
{
    public interface ILayoutContentProvider
    {
        Task<ILayoutContent> Get(string name);
        Task<bool> Delete(string name);
        Task<bool> Save(string name);
        Task<bool> Reset(string name);
        Task<List<ILayoutContent>> GetAll();
    }
}