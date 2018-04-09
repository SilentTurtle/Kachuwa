using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Kachuwa.Web.Theme
{
    public class ThemeManager
    {
        private IThemeConfig _configuration;

        private static readonly Lazy<ThemeManager> ThemeManagerInstance =
           new Lazy<ThemeManager>(() => new ThemeManager(new ThemeConfiguration()));

        public static ThemeManager Instance
        {
            get { return ThemeManagerInstance.Value; }
        }

        public void SetDefault(Action<IThemeConfig> configurator)
        {
            configurator(_configuration);
            //ViewEngines.Engines.Clear();
            //ViewEngines.Engines.Add(new ThemeableRazorViewEngine(_configuration));
        }
        public ThemeManager(IThemeConfig configuration)
        {
            _configuration = configuration;
        }


        public bool Install()
        {
            return true;
        }

        public bool UnInstall()
        {
            return true;
        }

        public void SetThemeResolver(IThemeResolver resolver)
        {

        }
    }
}