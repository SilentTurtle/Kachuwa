using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Kachuwa.Web.Theme
{
    public static class ThemeExtensions
    {
        public static void UseThemes(IApplicationBuilder builder, Action<IThemeConfig> configuration)
        {
            //ThemeManager.Instance.SetDefault(config =>
            //    {
            //        config.Directory = "~/Themes";
            //        config.FrontendThemeName = "Default";
            //        config.BackendThemeName = "Novoli";
            //        config.ThemeResolver = new DefaultThemeResolver();
            //    }
            //);
            ThemeManager.Instance.SetThemeResolver(new DefaultThemeResolver());
        }
        public static IServiceCollection RegisterThemeService(this IServiceCollection service, ThemeConfiguration configuration)
        {
            service.Configure<RazorViewEngineOptions>(options =>
            {
                options.ViewLocationExpanders.Add(new ThemeLocationExpander());
                options.ViewLocationExpanders.Add(new ComponentViewLocationExpander());

            });
            service.TryAddSingleton<IThemeConfig>(configuration);
            service.TryAddSingleton<IThemeResolver, DefaultThemeResolver>();
            return service;
        }
    }
}