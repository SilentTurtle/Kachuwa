using System.Threading.Tasks;

namespace Kachuwa.Job
{
    public interface IKachuwaJobEngineStarter
    {
        Task Start();
        Task Stop();
    }
}