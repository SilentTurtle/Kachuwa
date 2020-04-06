namespace Kachuwa.Web.Theme
{
    public interface IThemeConfig
    {
        string Directory { get; set; }
        string LayoutName { get; set; }
        IThemeResolver ThemeResolver { get; set; }
    }
}