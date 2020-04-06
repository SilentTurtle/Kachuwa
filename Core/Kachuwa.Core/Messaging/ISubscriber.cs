using System;
using System.Threading.Tasks;

namespace Kachuwa.Messaging
{
    public interface ISubscriber
    {
        //Task<Guid> Register(IMessageHub hub);
        Task<bool> Unsubscribe();
    }
}