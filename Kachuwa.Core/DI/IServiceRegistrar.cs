using Microsoft.Extensions.DependencyInjection;

namespace Kachuwa.Core.DI
{
    public interface IServiceRegistrar
    {
        void Register(IServiceCollection serviceCollection);
        void Update(IServiceCollection serviceCollection);

    }
}