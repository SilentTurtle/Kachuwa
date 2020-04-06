namespace Kachuwa.Web.Theme
{
    public class ThemeConfiguration : IThemeConfig
    {
        public ThemeConfiguration()
        {
            LayoutName = "_Layout";
            Directory = "Themes";
            ThemeResolver = new DefaultThemeResolver();
        }
        public string Directory { get; set; }
        public string LayoutName { get; set; }
        public IThemeResolver ThemeResolver { get; set; }
    }
}