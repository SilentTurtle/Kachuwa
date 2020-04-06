namespace Kachuwa.Web.Layout
{
    public interface ILayoutRenderer
    {
        string Render(LayoutContent layoutContent , LayoutGridSystem gridSystem);
    }
}