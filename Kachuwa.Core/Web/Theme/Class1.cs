using System;
using System.Collections;
using System.Text;
using Kachuwa.Tenant;
using Microsoft.Extensions.DependencyInjection;

namespace Kachuwa.Web.Theme
{
    class Class1
    {
    }

    public interface IThemeBuilder
    {

    }

    public interface IThemeComponent
    {

    }


    public class BootstrapThemeBuilder : IThemeBuilder
    {

    }

    public static class ThemeHelper
    {
        public static string GetCurrentTheme()
        {
            var context=ContextResolver.Context;
            var theme = (IThemeConfig)context.Items["Theme"];
            var currentTenant = (CurrentTenant)context.Items[TenantConstant.TenantContextKey];
            
            //temporary for single site
            if (theme != null)
            {
                return $"{currentTenant.Info.Name}/{theme}";//theme.ToString();
            }
            else
            {
                return $"{currentTenant.Info.Name}/{currentTenant.Info.ThemeConfig.FrontendThemeName}";
            }
            
        }
    }
}
