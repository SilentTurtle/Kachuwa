using System.Collections.Generic;
using System.Threading.Tasks;
using Kachuwa.Data;

namespace Kachuwa.RTC
{
    public interface IRTCUserService: IRTCConnectionManager
    {
        CrudService<RTCUser> CrudService { get; set; }

    }
}