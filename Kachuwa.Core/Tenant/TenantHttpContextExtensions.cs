using Microsoft.AspNetCore.Http;

namespace Kachuwa.Tenant
{
    public static class TenantHttpContextExtensions
    {
        private const string TenantContextKey = "KachuwaCurrentTenant";

        public static void SetCurrentTenant(this HttpContext context,CurrentTenant currentTenant)
        {
            context.Items[TenantContextKey] = currentTenant;
        }

        public static CurrentTenant GetCurrentTenant(this HttpContext context)
        {
            object tenantContext;
            if (context.Items.TryGetValue(TenantContextKey, out tenantContext))
            {
                return tenantContext as CurrentTenant;
            }

            return null;
        }

        public static Tenant GetTenant(this HttpContext context)
        {
            var currentTenant = GetCurrentTenant(context);

            if (currentTenant != null)
            {
                return currentTenant.Info;
            }

            return null;
        }
    }
}