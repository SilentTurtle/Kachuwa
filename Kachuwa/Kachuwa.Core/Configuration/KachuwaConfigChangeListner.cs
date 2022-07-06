using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Kachuwa.Configuration
{
    public class KachuwaConfigChangeListner : IConfigChangeListner
    {
        private readonly IHostApplicationLifetime _applicationLifetime;

        public KachuwaConfigChangeListner(IHostApplicationLifetime applicationLifetime)
        {
            _applicationLifetime = applicationLifetime;
        }
        public async Task<bool> Update()
        {
            _applicationLifetime.StopApplication();
            return true;
        }
    }
}