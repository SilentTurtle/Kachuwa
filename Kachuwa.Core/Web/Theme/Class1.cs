using System;
using System.Collections;
using System.Text;
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
          

            var themeconfig = context.RequestServices.GetService<IThemeConfig>();
            //temporary for single site
            if (theme != null)
            {
                return theme.ToString();

            }
            else
            {
                return themeconfig.FrontendThemeName;
            }
            
        }
    }
}
