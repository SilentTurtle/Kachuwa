using System.Threading.Tasks;

namespace Kachuwa.Caching
{
    public interface ICacheConfig
    {
        Task Init();
        Task Terminate();
    }
}