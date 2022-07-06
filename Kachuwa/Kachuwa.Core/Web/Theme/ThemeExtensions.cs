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
           // ThemeManager.Instance.SetThemeResolver(new DefaultThemeResolver());
        }
        /// <summary>
        /// Register Themes services with out tenant 
        /// </summary>
        /// <param name="service"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterThemeService(this IServiceCollection service, Action<IThemeConfig> configuration)
        {
            service.Configure<RazorViewEngineOptions>(options =>
            {
                options.ViewLocationExpanders.Add(new ThemeLocationExpander());
                options.ViewLocationExpanders.Add(new ComponentViewLocationExpander());

            });
            var themeConfig = new ThemeConfiguration();
            configuration(themeConfig);
            service.TryAddSingleton<IThemeConfig>(themeConfig);
            service.TryAddSingleton<IThemeResolver, DefaultThemeResolver>();
            service.AddScoped<IThemeManager, ThemeManager>();
            return service;
        }

        /// <summary>
        /// Regiser themes services for adaptation of tenant
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterThemeService(this IServiceCollection service)
        {
            service.Configure<RazorViewEngineOptions>(options =>
            {
                options.ViewLocationExpanders.Add(new ThemeLocationExpander());
                options.ViewLocationExpanders.Add(new ComponentViewLocationExpander());

            });
            service.TryAddSingleton<IThemeResolver, DefaultThemeResolver>();
            return service;
        }
    }
}