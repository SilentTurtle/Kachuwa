namespace Kachuwa.Web.Theme
{
    public class ThemeConfiguration : IThemeConfig
    {
        public ThemeConfiguration()
        {
            FrontendThemeName = "Default";
            BackendThemeName = "Novoli";
            LayoutName = "_Layout";
            ThemeResolver = new DefaultThemeResolver();
        }


        public string Directory { get; set; }
        public string FrontendThemeName { get; set; }
        public string BackendThemeName { get; set; }
        public string LayoutName { get; set; }
        public IThemeResolver ThemeResolver { get; set; }
    }
}