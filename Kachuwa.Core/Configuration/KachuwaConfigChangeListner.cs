using System.Threading.Tasks;

namespace Kachuwa.Configuration
{
    public class KachuwaConfigChangeListner : IConfigChangeListner
    {
        public async Task<bool> Update()
        {
            return true;
        }
    }
}