using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace Kachuwa.Configuration
{
    public class KachuwaConfigChangeListner : IConfigChangeListner
    {
        private readonly IApplicationLifetime _applicationLifetime;

        public KachuwaConfigChangeListner(IApplicationLifetime applicationLifetime)
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