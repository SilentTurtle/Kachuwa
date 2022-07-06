using System.Threading.Tasks;

namespace Kachuwa.Configuration
{
    public interface IConfigChangeListner
    {
        Task<bool> Update();
    }
}