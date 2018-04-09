using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Kachuwa.Storage
{
    public static class StorageExtensions
    {
        public static void UseKachuwaStorage(this IApplicationBuilder builder,IFileOptions options)
        {
            //builder.

        }
        public static IServiceCollection RegisterKachuwaStorageService(this IServiceCollection service, IFileOptions options)
        {
            return service.AddSingleton(options.StorageProvider);
        }
    }
}