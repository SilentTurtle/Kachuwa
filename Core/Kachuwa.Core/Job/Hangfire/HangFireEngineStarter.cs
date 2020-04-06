using System.Threading.Tasks;

namespace Kachuwa.Job
{
    public class HangFireEngineStarter : IKachuwaJobEngineStarter
    {
        public Task Start()
        {
            return Task.FromResult(1);
        }

        public Task Stop()
        {
            return Task.FromResult(1);
        }
    }
}