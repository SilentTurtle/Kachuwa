using System.Threading.Tasks;

namespace Kachuwa.Messaging
{
    public interface IPublisher
    {
        Task Publish();
        Task Publish<T>(T message);
    }
}