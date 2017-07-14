using Kachuwa.Caching;
using Kachuwa.Storage;

namespace Kachuwa.Tenant
{
    public class TenantConfig
    {
        public ICacheService Cache { get; set; }
        public IStorageProvider StorageProvider { get; set; }
    }
}