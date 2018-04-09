using Microsoft.AspNetCore.Mvc;

namespace Kachuwa.Web.Theme
{
    public interface IThemeResolver
    {
        string Resolve(ControllerContext controllerContext, string theme);
    }
}