using Kachuwa.AntiVirus;
using Kachuwa.Caching;
using Kachuwa.Core.AntiVirus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Kachuwa.Core.Extensions
{
    public static class KachuwaSetUpServiceExtensions
    {
        public static IServiceCollection UseDefaultMemoryCache(this IServiceCollection services)
        {
            services.TryAddSingleton<ICacheService, DefaultCacheService>();
            return services;
        }
        public static IServiceCollection UseRedisCache(this IServiceCollection services)
        {
            services.TryAddSingleton<ICacheService, RedisCacheService>();
            return services;
        }

        public static IServiceCollection UseWindowsDefenderScanner(this IServiceCollection services)
        {
            services.TryAddSingleton<IVirusScanner, WindowsDefenderScanner>();
            return services;
        }
        public static IServiceCollection UseAvgAntiVirusScanner(this IServiceCollection services)
        {
            services.TryAddSingleton<IVirusScanner, AVGScanner>();
            return services;
        }
        public static IServiceCollection UseEsetVirusScanner(this IServiceCollection services)
        {
            services.TryAddSingleton<IVirusScanner, EsetScanner>();
            return services;
        }

    }
}