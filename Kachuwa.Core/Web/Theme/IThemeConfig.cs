namespace Kachuwa.Web.Theme
{
    public interface IThemeConfig
    {
        string Directory { get; set; }
        string FrontendThemeName { get; set; }
        string BackendThemeName { get; set; }
        string LayoutName { get; set; }
        IThemeResolver ThemeResolver { get; set; }
    }
}